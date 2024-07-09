using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace PSVitaNPDRMDecryptor;

partial class Program
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
			form.SetFormState(false);
			Application.EnableVisualStyles();
			window.ShowDialog();
		})).Start();

		int i = 0;
		float maxProgress = o.InputFolders.Count();
		if (o.InputFolders.Any()) window.ShowProgress();

		List<string> exported = new List<string>();
		foreach (string dir in o.InputFolders)
		{
			if (window.Canceled) break;

			// Paths
			string inputDirTrimmed = dir.TrimEnd(Path.DirectorySeparatorChar);
			string workbin = inputDirTrimmed + "\\sce_sys\\package\\work.bin";
			string titleID = GetContentID(inputDirTrimmed + "\\sce_sys\\param.sfo", true);
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
				try { Directory.CreateDirectory(outputDir); }
				catch (Exception e) { MessageBox.Show("Could not create output directory " + o.OutputDir + ": " + e.Message); }
			}
			else
			{
				DeleteDirectory(outputDir, true);
				try { Directory.CreateDirectory(outputDir); }
				catch (Exception e) { MessageBox.Show("Could not create output directory " + o.OutputDir + ": " + e.Message); }
			}

			window.SetDecodingText(titleID);
			window.SetDecodingPhase("Decrypting PFS");
			window.Update(++i / maxProgress);

			DecryptPFS(inputDirTrimmed, outputDir, GetKLicensee(workbin));

			// Check if ELFs should be compressed
			string compressCommand = "";
			if (o.CompressELFs) compressCommand = " --compress";

			string[] files = Directory.GetFiles(outputDir, "*", SearchOption.AllDirectories);
			foreach (string SELF in files)
			{
				if (!IsSELF(SELF))
					continue;

				window.SetDecodingPhase("Decrypting " + Path.GetFileName(SELF));
				string tmpELF = SELF + ".elf";
				UnSELF(SELF, tmpELF, workbin);

				ReadSELFHeader(SELF, out byte[] PIH, out byte[] NPDRM, out byte[] bootParams);
				DeleteFile(SELF);	//Delete old eboot

				MakeFSELF(tmpELF, SELF, compressCommand);
				DeleteFile(tmpELF);

				WriteSELFHeader(SELF, PIH, NPDRM, bootParams);
			}

			window.SetDecodingPhase("Patching param.sfo");
			PatchParamSFO(paramsfo);

			// Use retail livearea for game demos
			if (Directory.Exists(retailLivearea))
			{
				DeleteDirectory(livearea, true);			//Directory.Move(retaillivearea, outputDir + "\\sce_sys\\sce_sys");
				Directory.Move(retailLivearea, livearea);   //MoveCMD(outputDir + "\\sce_sys\\sce_sys", outputDir);
				DeleteDirectory(retail, true);
			}

			DeleteFile(clearsign);
			DeleteFile(keystone);

			if (o.MakeVPK)
			{
				window.SetDecodingPhase("Making VPK");
				DeleteFile(outputDir + ".vpk");
				ZipFile.CreateFromDirectory(outputDir, outputDir + ".vpk", CompressionLevel.Optimal, false);
				DeleteDirectory(outputDir, true);
			}
		}

		if (window.Visible) window.BeginInvoke(new Action(() =>
		{
			form.SetFormState(true);
			window.AllowClose = true;
			window.Close();
		}));
	}
}