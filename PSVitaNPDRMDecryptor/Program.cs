﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

                /// Paths
                string inputDirTrimmed = inputDir.TrimEnd(Path.DirectorySeparatorChar);
                string dirName = Path.GetFileName(inputDirTrimmed);
                string outputDir = o.OutputDir.TrimEnd(Path.DirectorySeparatorChar) + "\\" + dirName;
                if (o.AddSuffix) outputDir += "_dec";   //Check if suffixs are needed
                string workbin = inputDirTrimmed + "\\sce_sys\\package\\work.bin";
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
                    DirectoryInfo di = new DirectoryInfo(outputDir);
                    RecursiveDelete(di, true);
                }

                window.SetDecodingText(dirName);
                window.Update(++i / maxProgress);

                // Read klicensee from work.bin
                string klic = "";
                using (FileStream fs = new FileStream(workbin, FileMode.Open))
				{
                    byte[] arr = new byte[16];
                    fs.Seek(80, SeekOrigin.Begin);
                    fs.Read(arr, 0, 16);
					fs.Close();
                    klic = ByteArrayToString(arr);
                }

                // Decrypt NPDRM contents
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "bin\\psvpfsparser-win64.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = "-k " + klic + " -i \"" + inputDir + "\" -o \"" + outputDir + "\""
                };
                Process p = Process.Start(psi);
                p.WaitForExit();

                // Check if ELFs should be compressed
                string compressCommand = "";
                if (o.CompressELFs) compressCommand = " --compress";

                string[] files = Directory.GetFiles(outputDir, "*.*", SearchOption.AllDirectories);
                foreach (string SELF in files)
                {
                    if (Path.GetFileName(SELF) == "right.suprx")
                        continue;

                    byte[] header = new byte[4];
                    byte[] magic = new byte[4] { 0x53, 0x43, 0x45, 0x00 };
                    using (FileStream fs = new FileStream(SELF, FileMode.Open))
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.Read(header, 0, 4);
                        fs.Close();
                        if (!header.SequenceEqual(magic))
                            continue;
                    }

                    string tmpELF = SELF + ".elf";

                    // Encrypted NPDRM SELF -> ELF
                    psi = new ProcessStartInfo
                    {
                        FileName = "bin\\self2elf.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = "-i \"" + SELF + "\" -o \"" + tmpELF + "\" -k \"" + workbin + "\""
                    };
                    p = Process.Start(psi);
                    p.WaitForExit();

                    // Read Program Identification Header
                    byte[] pih = new byte[0x20];
                    using (FileStream fs = new FileStream(SELF, FileMode.Open))
                    {
                        fs.Seek(0x80, SeekOrigin.Begin);
                        fs.Read(pih, 0, 0x20);
                        fs.Close();
                    }

                    // Read Boot Params
                    byte[] pattern = new byte[0x08] { 0x06, 0x00, 0x00, 0x00, 0x10, 0x01, 0x00, 0x00 };
                    byte[] tmpSELFArr = File.ReadAllBytes(SELF);
                    int offset = PatternAt(tmpSELFArr, pattern).FirstOrDefault();
                    byte[] bootParams = new byte[0x110];
                    using (FileStream fs = new FileStream(SELF, FileMode.Open))
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        fs.Read(bootParams, 0, 0x110);
                        fs.Close();
                    }

                    //Delete old eboot
                    File.Delete(SELF);

                    // ELF -> fSELF
                    psi = new ProcessStartInfo
                    {
                        FileName = "bin\\vdsuite-pubprx.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = "\"" + tmpELF + "\" \"" + SELF + "\"" + compressCommand
                    };
                    p = Process.Start(psi);
                    p.WaitForExit();

                    // Write back original SELF Program Identification Header
                    using (FileStream fs = new FileStream(SELF, FileMode.Open))
                    {
                        fs.Seek(0x80, SeekOrigin.Begin);
                        fs.Write(pih, 0, 0x20);
                        fs.Close();
                    }

                    // Write back original ELF Boot Params
                    tmpSELFArr = File.ReadAllBytes(SELF);
                    offset = PatternAt(tmpSELFArr, pattern).FirstOrDefault();
                    using (FileStream fs = new FileStream(SELF, FileMode.Open))
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        fs.Write(bootParams, 0, 0x110);
                        fs.Close();
                    }

                    File.Delete(tmpELF);
                }

                File.Delete(clearsign);
                File.Delete(keystone);
            }

			if (window.Visible) window.BeginInvoke(new Action(() => {
				window.AllowClose = true;
				window.Close();
			}));
		}

        public static void RecursiveDelete(DirectoryInfo baseDir, bool isRootDir)
        {
            if (!baseDir.Exists)
                return;
            foreach (var dir in baseDir.EnumerateDirectories()) RecursiveDelete(dir, false);
            foreach (var file in baseDir.GetFiles())
            {
                file.IsReadOnly = false;
                file.Delete();
            }
            if (!isRootDir) baseDir.Delete();
        }

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
