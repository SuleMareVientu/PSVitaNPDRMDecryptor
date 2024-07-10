using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using static PSVitaNPDRMDecryptor.Program.Param;

namespace PSVitaNPDRMDecryptor;

partial class Program
{
	public static string ByteArrayToString(byte[] ba)
	{
		return BitConverter.ToString(ba).Replace("-", "");
	}

	public static int PatternAt(byte[] src, byte[] pattern)
	{
		int maxFirstCharSlot = src.Length - pattern.Length + 1;
		for (int i = 0; i < maxFirstCharSlot; i++)
		{
			if (src[i] != pattern[0])
				continue;

			if (src.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
				return i;
		}
		return -1;
	}

	public static void DeleteFile(string file)
	{
		if (File.Exists(file))
		{
			try { File.Delete(file); }
			catch (Exception e) { MessageBox.Show("Could not delete \"" + Path.GetFileName(file) + "\": " + e.Message); }
		}
		return;
	}

	public static void DeleteDirectory(string dir, bool recursive = true)
	{
		dir = dir.TrimEnd(Path.DirectorySeparatorChar);
		if (Directory.Exists(dir))
		{
			try { Directory.Delete(dir, recursive); }
			catch (Exception e) { MessageBox.Show("Could not delete \"" + Path.GetFileName(dir) + "\": " + e.Message); }
		}
		return;
	}

	public static void MoveDirectory(string source, string target)
	{
		var sourcePath = source.TrimEnd('\\', ' ');
		var targetPath = target.TrimEnd('\\', ' ');
		try
		{
			var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories).GroupBy(s => Path.GetDirectoryName(s));
			foreach (var folder in files)
			{
				var targetFolder = folder.Key.Replace(sourcePath, targetPath);
				Directory.CreateDirectory(targetFolder);
				foreach (var file in folder)
				{
					var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
					if (File.Exists(targetFile)) File.Delete(targetFile);
					File.Move(file, targetFile);
				}
			}
			Directory.Delete(source, true);
		}
		catch (Exception e) { MessageBox.Show("Could not move directory \"" + sourcePath + "\": " + e.Message); }
		return;
	}

	public static bool IsDirectoryEmpty(string dir)
	{
		var dirPath = dir.TrimEnd('\\', ' ');
		if (!Directory.Exists(dirPath)) return false;
		string[] files = null;
		string[] dirs = null;
		try
		{
			files = Directory.GetFiles(dirPath, "*", SearchOption.TopDirectoryOnly);
			dirs = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
			if ((files != null || dirs != null) && (files.Length > 0 || dirs.Length > 0))
				return false;
		}
		catch (Exception) { }
		return true;
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

	public const string psvpfsparser = "bin\\psvpfsparser-win64.exe";
	public static void DecryptPFS(string inputDir, string outputDir, string klic)
	{
		if (!File.Exists(psvpfsparser))
		{
			MessageBox.Show("\"psvpfsparser-win64.exe\" was not found or is inaccessible.");
			Application.Exit();
			return;
		}

		inputDir = inputDir.TrimEnd(Path.DirectorySeparatorChar);
		outputDir = outputDir.TrimEnd(Path.DirectorySeparatorChar);

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

		SELF = SELF.TrimEnd(Path.DirectorySeparatorChar);
		ELF = ELF.TrimEnd(Path.DirectorySeparatorChar);

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

		ELF = ELF.TrimEnd(Path.DirectorySeparatorChar);
		SELF = SELF.TrimEnd(Path.DirectorySeparatorChar);

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
		SELF = SELF.TrimEnd(Path.DirectorySeparatorChar);
		if (Path.GetFileName(SELF) == "right.suprx")
			return false;

		const int magic = 0x00454353;   // 53 43 45 00
		try
		{
			using (BinaryReader br = new BinaryReader(File.OpenRead(SELF)))
			{
				int tmp = br.ReadInt32();
				br.Close();
				if (tmp == magic)
					return true;
			}
		}
		catch (EndOfStreamException) { }
		catch (Exception e) { MessageBox.Show("Could not read SELF: " + e.Message); }

		return false;
	}

	public static string GetKLicensee(string workbin)
	{
		string klic = "";
		workbin = workbin.TrimEnd(Path.DirectorySeparatorChar);
		try
		{
			using (BinaryReader br = new BinaryReader(File.OpenRead(workbin)))
			{
				byte[] arr = new byte[0x10];
				br.BaseStream.Seek(0x50, SeekOrigin.Begin);
				br.Read(arr, 0, 0x10);
				br.Close();
				klic = ByteArrayToString(arr);
			}
		}
		catch (Exception e) { MessageBox.Show("Could not read work.bin: " + e.Message); }
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
		SELF = SELF.TrimEnd(Path.DirectorySeparatorChar);

		// Read SELF
		const int maxSELFRead = 0x2710;
		byte[] SELFarr = new byte[maxSELFRead];
		try
		{
			using (BinaryReader br = new BinaryReader(File.OpenRead(SELF)))
			{ br.BaseStream.Read(SELFarr, 0, maxSELFRead); br.Close(); }

			using (BinaryReader br = new BinaryReader(File.OpenRead(SELF)))
			{
				// Read Program Identification Header
				br.BaseStream.Seek(0x80, SeekOrigin.Begin);
				br.Read(PIH, 0, 0x20);

				// Read NPDRM Header
				int offset = PatternAt(SELFarr, NPDRMMagic);
				if (offset >= 0)
				{
					br.BaseStream.Seek(offset, SeekOrigin.Begin);
					br.Read(NPDRM, 0, 0x110);
				}

				// Read Boot Params
				offset = PatternAt(SELFarr, bootParamsMagic);
				if (offset >= 0)
				{
					br.BaseStream.Seek(offset, SeekOrigin.Begin);
					br.Read(bootParams, 0, 0x110);
				}
				br.Close();
			}
		}
		catch (Exception e) { MessageBox.Show("Could not read SELF header: " + e.Message); }
		return;
	}

	public static void WriteSELFHeader(string SELF, byte[] PIH, byte[] NPDRM, byte[] bootParams)
	{
		// Read SELF
		const int maxSELFRead = 0x2710;
		byte[] SELFarr = new byte[maxSELFRead];
		SELF = SELF.TrimEnd(Path.DirectorySeparatorChar);

		try
		{
			using (BinaryReader br = new BinaryReader(File.OpenRead(SELF)))
			{ br.BaseStream.Read(SELFarr, 0, maxSELFRead); br.Close(); }

			using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(SELF)))
			{
				// Read Program Identification Header
				bw.BaseStream.Seek(0x80, SeekOrigin.Begin);
				bw.Write(PIH, 0, 0x20);

				// Read NPDRM Header
				int offset = PatternAt(SELFarr, NPDRMMagic);
				if (offset >= 0 && NPDRM != null)
				{
					bw.BaseStream.Seek(offset, SeekOrigin.Begin);
					bw.Write(NPDRM, 0, 0x110);
					bw.BaseStream.Seek(offset + 0x18, SeekOrigin.Begin);
					bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 0x04);   // 0x00 DRM Type Unknown (official name) // 0x0D Free (PSP2/PSM)
				}

				// Read Boot Params
				offset = PatternAt(SELFarr, bootParamsMagic);
				if (offset >= 0 && bootParams != null)
				{
					bw.BaseStream.Seek(offset, SeekOrigin.Begin);
					bw.Write(bootParams, 0, 0x110);
				}
				bw.Close();
			}
		}
		catch (Exception e) { MessageBox.Show("Could not write SELF header: " + e.Message); }
		return;
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
		sfoFile = sfoFile.TrimEnd(Path.DirectorySeparatorChar);
		if (!File.Exists(sfoFile)) return;
		try
		{
			using (BinaryReader br = new BinaryReader(File.OpenRead(sfoFile)))
			{
				const int PSFMagic = 0x46535000;    // 00 50 53 46
				if (br.ReadInt32() != PSFMagic)
				{
					br.Close();
					return;
				}

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
		catch (Exception e) { MessageBox.Show("Could not load SFO: " + e.Message); }
	}

	public static bool PatchParamSFO(string paramsfo)
	{
		// Clear "Application is upgradable" bit in param.sfo "ATTRIBUTE"
		bool status = false;
		paramsfo = paramsfo.TrimEnd(Path.DirectorySeparatorChar);
		LoadSFO(paramsfo);
		foreach (Param param in paramList)
		{
			if (param.name != "ATTRIBUTE") continue;
			try
			{
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
			catch (Exception e) { MessageBox.Show("Could not patch SFO: " + e.Message); }
		}
		return status;
	}

	public static string GetContentID(string paramsfo, bool onlyTitleID = false)
	{
		LoadSFO(paramsfo);
		foreach (Param param in paramList)
		{
			if (param.name == "CONTENT_ID")
			{
				if (onlyTitleID) return param.dataString.Remove(0, 7).Remove(9);
				return param.dataString;
			}	
		}
		return "";
	}

	public static string GetCategory(string paramsfo)
	{
		LoadSFO(paramsfo);
		foreach (Param param in paramList)
		{
			if (param.name == "CATEGORY")
				return param.dataString;
		}
		return "";
	}
}