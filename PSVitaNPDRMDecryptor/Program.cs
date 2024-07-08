using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace PSVitaNPDRMDecryptor {
    class Program {
        public static OptionsForm form;
        [STAThread]
		static void Main(string[] args) {
			Application.EnableVisualStyles();
            Application.Run(form = new OptionsForm());
            Task.WaitAll(form.RunningTasks.ToArray());
        }

        /// <summary>
        /// Runs a batch conversion process.
        /// </summary>
        /// <param name="o">Options for the batch.</param>
		public static void Run(Options o) {
			MultipleProgressWindow window = new MultipleProgressWindow();
			new Thread(new ThreadStart(() => {
				Application.EnableVisualStyles();
				window.ShowDialog();
			})).Start();

			if (!o.InputFolders.Any()) {
				MessageBox.Show("No input folders were selected.");
			}

			int i = 0;
			float maxProgress = o.InputFolders.Count();
			if (o.InputFolders.Any()) window.ShowProgress();

			List<string> exported = new List<string>();
			foreach (string inputDir in o.InputFolders) {
				if (window.Canceled) break;

                // Paths
                string inputDirTrimmed = inputDir.TrimEnd(Path.DirectorySeparatorChar);
                string workbin = inputDirTrimmed + "\\sce_sys\\package\\work.bin";
                string dirName = Path.GetFileName(inputDirTrimmed);
                string outputDir = Path.GetFullPath(o.OutputDir.TrimEnd(Path.DirectorySeparatorChar) + "\\" + dirName);
                if (o.AddSuffix) outputDir += "_dec";   //Check if suffix is needed
                string paramsfo = outputDir + "\\sce_sys\\param.sfo";
                string livearea = outputDir + "\\sce_sys\\livearea";
                string retail = outputDir + "\\sce_sys\\retail";
                string retailLivearea = outputDir + "\\sce_sys\\retail\\livearea";
                string clearsign = outputDir + "\\sce_sys\\clearsign";
                string keystone = outputDir + "\\sce_sys\\keystone";

                if (!Directory.Exists(outputDir)) {
					try {
						Directory.CreateDirectory(outputDir);
					} catch (Exception e) {
						MessageBox.Show("Could not create output directory " + o.OutputDir + ": " + e.Message);
					}
				}
                else
                {
                    Directory.Delete(outputDir, true);
                    Directory.CreateDirectory(outputDir);
                }

                window.SetDecodingText(dirName);
                window.Update(++i / maxProgress);

                DecryptPFS(inputDir, outputDir, GetKLicensee(workbin));
                
                // Check if ELFs should be compressed
                string compressCommand = "";
                if (o.CompressELFs) compressCommand = " --compress";

                string[] files = Directory.GetFiles(outputDir, "*", SearchOption.AllDirectories);
                foreach (string SELF in files)
                {
                    if (!IsSELF(SELF.TrimEnd(Path.DirectorySeparatorChar)))
                        continue;

                    string tmpELF = SELF + ".elf";
                    UnSELF(SELF, tmpELF, workbin);

                    ReadSELFHeader(SELF, out byte[] PIH, out byte[] NPDRM, out byte[] bootParams);
                    File.Delete(SELF);  //Delete old eboot

                    MakeFSELF(tmpELF, SELF, compressCommand);
                    File.Delete(tmpELF);

                    WriteSELFHeader(SELF, PIH, NPDRM, bootParams);
                }

                PatchParamSfo(paramsfo);

                // Use retail livearea for game demos
                if (Directory.Exists(retailLivearea))
                {
                    Directory.Delete(livearea, true);           //Directory.Move(retaillivearea, outputDir + "\\sce_sys\\sce_sys");
                    Directory.Move(retailLivearea, livearea);   //MoveCMD(outputDir + "\\sce_sys\\sce_sys", outputDir);
                    Directory.Delete(retail, true);
                }

                File.Delete(clearsign);
                File.Delete(keystone);

                if (o.MakeVPK)
                {
                    File.Delete(outputDir + ".vpk");
                    ZipFile.CreateFromDirectory(outputDir, outputDir + ".vpk");
                    Directory.Delete(outputDir, true);
                }
            }

			if (window.Visible) window.BeginInvoke(new Action(() => {
				window.AllowClose = true;
				window.Close();
			}));
		}

        public const string psvpfsparser = "bin\\psvpfsparser-win64.exe";
        public static void DecryptPFS(string inputDir, string outputDir, string klic)
        {
            if (!File.Exists(psvpfsparser))
            {
                MessageBox.Show("\"psvpfsparser-win64.exe\" was not found or is inaccessible");
                Application.Exit();
                return;
            }

            // Decrypt NPDRM contents
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = psvpfsparser,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "-k " + klic + " -i \"" + inputDir + "\" -o \"" + outputDir + "\""
            };
            Process p = Process.Start(psi);
            p.WaitForExit();
        }

        public const string self2elf = "bin\\self2elf.exe";
        public static void UnSELF(string SELF, string ELF, string workbin)
        {
            if (!File.Exists(self2elf))
            {
                MessageBox.Show("\"self2elf.exe\" was not found or is inaccessible");
                Application.Exit();
                return;
            }

            // Encrypted NPDRM SELF -> ELF
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = self2elf,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "-i \"" + SELF + "\" -o \"" + ELF + "\" -k \"" + workbin + "\""
            };
            Process p = Process.Start(psi);
            p.WaitForExit();
        }

        public const string vdsuitepubprx = "bin\\vdsuite-pubprx.exe";
        public static void MakeFSELF(string ELF, string SELF, string commands)
        {
            if (!File.Exists(vdsuitepubprx))
            {
                MessageBox.Show("\"vdsuite-pubprx.exe\" was not found or is inaccessible");
                Application.Exit();
                return;
            }

            // ELF -> fSELF
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = vdsuitepubprx,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "\"" + ELF + "\" \"" + SELF + "\"" + commands
            };
            Process p = Process.Start(psi);
            p.WaitForExit();
        }

        public static bool IsSELF(string SELF)
        {
            if (Path.GetFileName(SELF) == "right.suprx")
                return false;

            byte[] header = new byte[4];
            byte[] magic = new byte[4] { 0x53, 0x43, 0x45, 0x00 };
            using (FileStream fs = new FileStream(SELF, FileMode.Open))
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(header, 0, 4);
                fs.Close();
                if (header.SequenceEqual(magic))
                    return true;
            }
            return false;
        }

        public static string GetKLicensee(string workbin)
        {
            string klic = "";
            using (FileStream fs = new FileStream(workbin, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] arr = new byte[0x10];
                fs.Seek(0x50, SeekOrigin.Begin);
                fs.Read(arr, 0, 0x10);
                fs.Close();
                klic = ByteArrayToString(arr);
            }
            return klic;
        }

        // Constants
        public static byte[] NPDRMMagic = new byte[0x08] { 0x05, 0x00, 0x00, 0x00, 0x10, 0x01, 0x00, 0x00 };
        public static byte[] bootParamsMagic = new byte[0x08] { 0x06, 0x00, 0x00, 0x00, 0x10, 0x01, 0x00, 0x00 };
        public static void ReadSELFHeader(string SELF, out byte[] PIH, out byte[] NPDRM, out byte[] bootParams)
        {
            PIH = new byte[0x20];
            NPDRM = new byte[0x110];
            bootParams = new byte[0x110];

            // Read SELF
            byte[] SELFarr = File.ReadAllBytes(SELF);
            using (FileStream fs = new FileStream(SELF, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read Program Identification Header
                fs.Seek(0x80, SeekOrigin.Begin);
                fs.Read(PIH, 0, 0x20);

                // Read NPDRM Header
                fs.Seek(PatternAt(SELFarr, Program.NPDRMMagic).FirstOrDefault(), SeekOrigin.Begin);
                fs.Read(NPDRM, 0, 0x110);

                // Read Boot Params
                fs.Seek(PatternAt(SELFarr, bootParamsMagic).FirstOrDefault(), SeekOrigin.Begin);
                fs.Read(bootParams, 0, 0x110);

                fs.Close();
            }
            return;
        }

        public static void WriteSELFHeader(string SELF, byte[] PIH, byte[] NPDRM, byte[] bootParams)
        {
            // Read SELF
            byte[] SELFarr = File.ReadAllBytes(SELF);
            using (FileStream fs = new FileStream(SELF, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                // Read Program Identification Header
                fs.Seek(0x80, SeekOrigin.Begin);
                fs.Write(PIH, 0, 0x20);

                // Read NPDRM Header
                int offset = PatternAt(SELFarr, NPDRMMagic).FirstOrDefault();
                fs.Seek(offset, SeekOrigin.Begin);
                fs.Write(NPDRM, 0, 0x110);
                fs.Seek(offset + 0x18, SeekOrigin.Begin);
                fs.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 0x04);    // 0x00 DRM Type Unknown (official name) // 0x0D Free (PSP2/PSM)

                // Read Boot Params
                fs.Seek(PatternAt(SELFarr, bootParamsMagic).FirstOrDefault(), SeekOrigin.Begin);
                fs.Write(bootParams, 0, 0x110);

                fs.Close();
            }
            return;
        }

        public static bool PatchParamSfo(string paramsfo)
        {
            bool status = false;
            // Clear "Application is upgradable" bit in param.sfo "ATTRIBUTE"
            using (FileStream fs = new FileStream(paramsfo, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                byte[] KeyTableStartOffset = new byte[0x04];
                byte[] ATTRIBUTE = new byte[0x04];
                fs.Seek(0x0C, SeekOrigin.Begin);
                fs.Read(KeyTableStartOffset, 0, 0x04); KeyTableStartOffset.Reverse();
                int AttributeOffsetInt = BitConverter.ToInt32(KeyTableStartOffset, 0) + 0x08;
                fs.Seek(AttributeOffsetInt, SeekOrigin.Begin);
                fs.Read(ATTRIBUTE, 0, 0x04); ATTRIBUTE.Reverse();
                int AttributeInt = BitConverter.ToInt32(ATTRIBUTE, 0);
                if ((AttributeInt & 1024) == 1024)
                {
                    ATTRIBUTE = BitConverter.GetBytes(AttributeInt & ~1024); ATTRIBUTE.Reverse();
                    fs.Seek(AttributeOffsetInt, SeekOrigin.Begin);
                    fs.Write(ATTRIBUTE, 0, 0x04);
                    status = true;
                }
                fs.Close();
            }
            return status;
        }

        /*
        public static void MoveCMD(string source, string target)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "/C robocopy \"" + source + "\" \"" + target + "\" /MOVE /E /R:100 /W:3"   // /COPYALL /DCOPY:DAT
                //Arguments = "/C SET COPYCMD=/Y && move \"" + source + "\" \"" + target + "\""
            };
            Process p = Process.Start(psi);
            p.WaitForExit();
        }
        */

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }
    }
}
