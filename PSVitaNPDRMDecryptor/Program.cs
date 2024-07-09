using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Text;
using static PSVitaNPDRMDecryptor.Program.Param;

namespace PSVitaNPDRMDecryptor;

class Program
{
	public static OptionsForm form;
	[STAThread]
	static void Main(string[] args)
	{
		Application.EnableVisualStyles();
		Application.Run(form = new OptionsForm());
		Task.WaitAll(form.RunningTasks.ToArray());
	}

	/// <summary>
	/// Runs a batch conversion process.
	/// </summary>
	/// <param name="o">Options for the batch.</param>
	public static void Run(Options o)
	{
		if (!o.InputFolders.Any())
		{
			MessageBox.Show("No input folders were selected.");
			return;
		}

		MultipleProgressWindow window = new MultipleProgressWindow();
		new Thread(new ThreadStart(() =>
		{
			Application.EnableVisualStyles();
			window.ShowDialog();
		})).Start();

		int i = 0;
		float maxProgress = o.InputFolders.Count();
		if (o.InputFolders.Any()) window.ShowProgress();

		List<string> exported = new List<string>();
		foreach (string inputDir in o.InputFolders)
		{
			if (window.Canceled) break;

			// Paths
			string inputDirTrimmed = inputDir.TrimEnd(Path.DirectorySeparatorChar);
			string workbin = inputDirTrimmed + "\\sce_sys\\package\\work.bin";
			string titleID = GetContentID(inputDirTrimmed + "\\sce_sys\\param.sfo").Remove(0, 7).Remove(9, 20);
			string dirName = Path.GetFileName(inputDirTrimmed);
			if (o.UseTitleID) dirName = titleID;
			string outputDir = Path.GetFullPath(o.OutputDir.TrimEnd(Path.DirectorySeparatorChar) + "\\" + dirName);
			if (o.AddSuffix) outputDir += "_dec";	//Check if suffix is needed
			string paramsfo = outputDir + "\\sce_sys\\param.sfo";
			string livearea = outputDir + "\\sce_sys\\livearea";
			string retail = outputDir + "\\sce_sys\\retail";
			string retailLivearea = outputDir + "\\sce_sys\\retail\\livearea";
			string clearsign = outputDir + "\\sce_sys\\clearsign";
			string keystone = outputDir + "\\sce_sys\\keystone";

			if (!Directory.Exists(outputDir))
			{
				try
				{
					Directory.CreateDirectory(outputDir);
				}
				catch (Exception e)
				{
					MessageBox.Show("Could not create output directory " + o.OutputDir + ": " + e.Message);
				}
			}
			else
			{
				Directory.Delete(outputDir, true);
				Directory.CreateDirectory(outputDir);
			}

			window.SetDecodingText(titleID);
			window.SetDecodingPhase("Decrypting PFS");
			window.Update(++i / maxProgress);

			DecryptPFS(inputDir, outputDir, GetKLicensee(workbin));

			// Check if ELFs should be compressed
			string compressCommand = "";
			if (o.CompressELFs) compressCommand = " --compress";
			window.SetDecodingPhase("Decrypting SELFs");

			string[] files = Directory.GetFiles(outputDir, "*", SearchOption.AllDirectories);
			foreach (string SELF in files)
			{
				if (!IsSELF(SELF.TrimEnd(Path.DirectorySeparatorChar)))
					continue;

				string tmpELF = SELF + ".elf";
				UnSELF(SELF, tmpELF, workbin);

				ReadSELFHeader(SELF, out byte[] PIH, out byte[] NPDRM, out byte[] bootParams);
				File.Delete(SELF);	//Delete old eboot

				MakeFSELF(tmpELF, SELF, compressCommand);
				File.Delete(tmpELF);

				WriteSELFHeader(SELF, PIH, NPDRM, bootParams);
			}

			PatchParamSFO(paramsfo);

			// Use retail livearea for game demos
			if (Directory.Exists(retailLivearea))
			{
				Directory.Delete(livearea, true);			//Directory.Move(retaillivearea, outputDir + "\\sce_sys\\sce_sys");
				Directory.Move(retailLivearea, livearea);	//MoveCMD(outputDir + "\\sce_sys\\sce_sys", outputDir);
				Directory.Delete(retail, true);
			}

			File.Delete(clearsign);
			File.Delete(keystone);

			if (o.MakeVPK)
			{
				window.SetDecodingPhase("Making VPK");
				File.Delete(outputDir + ".vpk");
				ZipFile.CreateFromDirectory(outputDir, outputDir + ".vpk", CompressionLevel.Optimal, false);
				Directory.Delete(outputDir, true);
			}
		}

		if (window.Visible) window.BeginInvoke(new Action(() =>
		{
			window.AllowClose = true;
			window.Close();
		}));
	}

	public const string psvpfsparser = "bin\\psvpfsparser-win64.exe";
	public static void DecryptPFS(string inputDir, string outputDir, string klic)
	{
		if (!File.Exists(psvpfsparser))
		{
			MessageBox.Show("\"psvpfsparser-win64.exe\" was not found or is inaccessible.");
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
			MessageBox.Show("\"self2elf.exe\" was not found or is inaccessible.");
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
			MessageBox.Show("\"vdsuite-pubprx.exe\" was not found or is inaccessible.");
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
		if (Path.GetFileName(SELF.TrimEnd(Path.DirectorySeparatorChar)) == "right.suprx")
			return false;

		const int magic = 0x00454353;	// 53 43 45 00
		using (BinaryReader br = new BinaryReader(File.Open(SELF, FileMode.Open, FileAccess.Read, FileShare.Read)))
		{
			int tmp = br.ReadInt32();
			br.Close();
			if (tmp == magic)
				return true;
		}
		return false;
	}

	public static string GetKLicensee(string workbin)
	{
		string klic = "";
		using (BinaryReader br = new BinaryReader(File.Open(workbin, FileMode.Open, FileAccess.Read, FileShare.Read)))
		{
			byte[] arr = new byte[0x10];
			br.BaseStream.Seek(0x50, SeekOrigin.Begin);
			br.Read(arr, 0, 0x10);
			br.Close();
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
		const int maxSELFRead = 0x2710;
		byte[] SELFarr = new byte[maxSELFRead];
		using (BinaryReader br = new BinaryReader(File.Open(SELF, FileMode.Open, FileAccess.Read, FileShare.Read)))
		{ br.BaseStream.Read(SELFarr, 0, maxSELFRead); br.Close(); }

		using (BinaryReader br = new BinaryReader(File.Open(SELF, FileMode.Open, FileAccess.Read, FileShare.Read)))
		{
			// Read Program Identification Header
			br.BaseStream.Seek(0x80, SeekOrigin.Begin);
			br.Read(PIH, 0, 0x20);

			// Read NPDRM Header
			br.BaseStream.Seek(PatternAt(SELFarr, Program.NPDRMMagic).FirstOrDefault(), SeekOrigin.Begin);
			br.Read(NPDRM, 0, 0x110);

			// Read Boot Params
			br.BaseStream.Seek(PatternAt(SELFarr, bootParamsMagic).FirstOrDefault(), SeekOrigin.Begin);
			br.Read(bootParams, 0, 0x110);
			br.Close();
		}
		return;
	}

	public static void WriteSELFHeader(string SELF, byte[] PIH, byte[] NPDRM, byte[] bootParams)
	{
		// Read SELF
		const int maxSELFRead = 0x2710;
		byte[] SELFarr = new byte[maxSELFRead];
		using (BinaryReader br = new BinaryReader(File.Open(SELF, FileMode.Open, FileAccess.Read, FileShare.Read)))
		{ br.BaseStream.Read(SELFarr, 0, maxSELFRead); br.Close(); }

		using (BinaryWriter bw = new BinaryWriter(File.Open(SELF, FileMode.Open, FileAccess.Write, FileShare.Read)))
		{
			// Read Program Identification Header
			bw.BaseStream.Seek(0x80, SeekOrigin.Begin);
			bw.Write(PIH, 0, 0x20);

			// Read NPDRM Header
			int offset = PatternAt(SELFarr, NPDRMMagic).FirstOrDefault();
			bw.BaseStream.Seek(offset, SeekOrigin.Begin);
			bw.Write(NPDRM, 0, 0x110);
			bw.BaseStream.Seek(offset + 0x18, SeekOrigin.Begin);
			bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 0x04);	// 0x00 DRM Type Unknown (official name) // 0x0D Free (PSP2/PSM)

			// Read Boot Params
			bw.BaseStream.Seek(PatternAt(SELFarr, bootParamsMagic).FirstOrDefault(), SeekOrigin.Begin);
			bw.Write(bootParams, 0, 0x110);
			bw.Close();
		}
		return;
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

	public class Param
	{
		public enum ParamFMT
		{
			UTF8Special = 0x0004,	// 04 00
			UTF8String = 0x0204,	// 04 02
			uint32 = 0x0404			// 04 04
		}

		public int keyTableOffset;
		public int dataTableOffset;
		public int indexTableEntries;

		public ushort keyOffset;
		public ushort paramFMT;
		public uint paramLen;
		public uint paramMaxLen;
		public uint dataOffset;

		public string name;
		public uint dataInt;
		public string dataString;

		public void Load(BinaryReader br)
		{
			keyOffset = br.ReadUInt16();
			paramFMT = br.ReadUInt16();
			paramLen = br.ReadUInt32();
			paramMaxLen = br.ReadUInt32();
			dataOffset = br.ReadUInt32();
		}
	}

	public static List<Param> paramList;

	public static void LoadSFO(string sfoFile)
	{
		if (!File.Exists(sfoFile))
			return;
		try
		{
			using (BinaryReader br = new BinaryReader(File.OpenRead(sfoFile)))
			{
				const int PSFMagic = 0x46535000;	// 00 50 53 46
				if (br.ReadInt32() != PSFMagic)
					throw new Exception("Not a SFO file!");

				int version = br.ReadInt32();
				int keyTableOffset = br.ReadInt32();
				int dataTableOffset = br.ReadInt32();
				int indexTableEntries = br.ReadInt32();
				paramList = new List<Param>();
				checked
				{
					for (int i = 0; i < indexTableEntries; i++)
					{
						Param param = new Param();
						param.keyTableOffset = keyTableOffset;
						param.dataTableOffset = dataTableOffset;
						param.indexTableEntries = indexTableEntries;
						br.BaseStream.Seek(20 + i * 16, SeekOrigin.Begin);
						param.Load(br);
						br.BaseStream.Seek(keyTableOffset + param.keyOffset, SeekOrigin.Begin);
						int keyLength = 0;
						while (br.ReadByte() != 0x00) { keyLength++; }
						br.BaseStream.Seek(keyTableOffset + param.keyOffset, SeekOrigin.Begin);
						param.name = Encoding.UTF8.GetString(br.ReadBytes(keyLength));

						br.BaseStream.Seek(dataTableOffset + param.dataOffset, SeekOrigin.Begin);
						if (param.paramFMT == (ushort)ParamFMT.uint32)
							param.dataInt = br.ReadUInt32();
						else if (param.paramFMT == (ushort)ParamFMT.UTF8String || param.paramFMT == (ushort)ParamFMT.UTF8Special)
							param.dataString = Encoding.UTF8.GetString(br.ReadBytes((int)param.paramLen - 1));

						paramList.Add(param);
					}
				}
				br.Close();
			}
		}
		catch (Exception ex) { }
	}

	public static bool PatchParamSFO(string paramsfo)
	{
		bool status = false;
		// Clear "Application is upgradable" bit in param.sfo "ATTRIBUTE"
		LoadSFO(paramsfo);
		foreach (Param param in paramList)
		{
			if (param.name != "ATTRIBUTE") continue;
			using (FileStream fs = new FileStream(paramsfo, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
			{
				byte[] ATTRIBUTE = new byte[0x04];
				var offset = param.dataTableOffset + param.dataOffset;
				fs.Seek(offset, SeekOrigin.Begin);
				fs.Read(ATTRIBUTE, 0, 0x04); ATTRIBUTE.Reverse();
				uint AttributeInt = BitConverter.ToUInt32(ATTRIBUTE, 0);
				if ((AttributeInt & 1024) == 1024)
				{
					ATTRIBUTE = BitConverter.GetBytes(AttributeInt & ~1024); ATTRIBUTE.Reverse();
					fs.Seek(offset, SeekOrigin.Begin);
					fs.Write(ATTRIBUTE, 0, 0x04);
					status = true;
				}
				fs.Close();
			}
		}
		return status;
	}

	public static string GetContentID(string paramsfo)
	{
		LoadSFO(paramsfo);
		foreach (Param param in paramList)
		{
			if (param.name == "CONTENT_ID")
				return param.dataString;
		}
		return "";
	}
}