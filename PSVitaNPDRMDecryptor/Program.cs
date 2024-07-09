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
}