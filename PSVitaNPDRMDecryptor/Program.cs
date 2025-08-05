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
	static void Main()
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

			if (o.ShowCMD) hideCMD = false;
			else hideCMD = true;

			#region app

			// Paths
			bool isApp = true;
			string inputDirTrimmed = dir.TrimEnd(Path.DirectorySeparatorChar);
			string workbin = inputDirTrimmed + "\\sce_sys\\package\\work.bin";
			string headbin = inputDirTrimmed + "\\sce_sys\\package\\head.bin";
			string titleID = GetContentID(inputDirTrimmed + "\\sce_sys\\param.sfo", true);
			string category = GetCategory(inputDirTrimmed + "\\sce_sys\\param.sfo").ToLower();
			string dirName = Path.GetFileName(inputDirTrimmed);
			if (o.UseTitleID)
			{
				dirName = titleID;
				if (category == "gp" || category == "gpc" || category == "gpd")
				{
					dirName += "_patch";
					isApp = false;
				}

				if (category == "ac")
				{
					string licenseID = GetContentID(inputDirTrimmed + "\\sce_sys\\param.sfo").Remove(0, 20);
					dirName = dirName + "_addcont" + "\\" + licenseID;
					isApp = false;
				}	
			}
			string outputBaseDir = Path.GetFullPath(o.OutputDir.TrimEnd(Path.DirectorySeparatorChar));
			string outputDir = outputBaseDir + "\\" + dirName;
			if (o.AddSuffix) outputDir += "_dec";   //Check if suffix is needed
			string outputHeadbinDir = outputDir + "\\sce_sys\\package";
			string outputHeadbin = outputDir + "\\sce_sys\\package\\head.bin";
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

            string klic = GetKLicensee(workbin);
            DecryptPFS(inputDirTrimmed, outputDir, klic);

			// Check if ELFs should be compressed
			string compressCommand = "";
			if (o.CompressELFs) compressCommand = "-c";

			string[] files = Directory.GetFiles(outputDir, "*", SearchOption.AllDirectories);
			foreach (string SELF in files)
			{
				if (!IsSELF(SELF))
					continue;

				window.SetDecodingPhase("Decrypting " + Path.GetFileName(SELF));
				string tmpELF = SELF + ".elf";
				UnSELF(SELF, tmpELF, klic);

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

			// Copy original head.bin if needed
			if (o.CopyHeadBin && File.Exists(headbin))
			{
				if (!Directory.Exists(outputHeadbinDir))
				{
					try { Directory.CreateDirectory(outputHeadbinDir); }
					catch (Exception e) { MessageBox.Show("Could not create \"head.bin\" output directory " + outputHeadbinDir + ": " + e.Message); }
				}
				try { File.Copy(headbin, outputHeadbin); }
				catch (Exception e) { MessageBox.Show("Could not copy \"head.bin\" to " + outputHeadbin + ": " + e.Message); }
			}

			DeleteFile(clearsign);
			DeleteFile(keystone);

			if (!isApp || !o.LookForAddcont) continue;

			#endregion

			#region patch

			if (window.Canceled) break;

			// Patch Paths
			string patchDir = Directory.GetParent(inputDirTrimmed).ToString().TrimEnd(Path.DirectorySeparatorChar) + "\\patch\\" + titleID;
			string outputPatchBaseDirSuffix = "\\patch";
			if (o.UseRePatch) outputPatchBaseDirSuffix = "\\rePatch";	//Use rePatch naming scheme 
			string outputPatchBaseDir = Path.GetFullPath(o.OutputDir.TrimEnd(Path.DirectorySeparatorChar) + outputPatchBaseDirSuffix);
			string outputPatchDir = outputPatchBaseDir + "\\" + titleID;
			if (o.AddSuffix) outputPatchDir += "_dec";   //Check if suffix is needed
			string patchParamsfo = outputPatchDir + "\\sce_sys\\param.sfo";
			string patchLivearea = outputPatchDir + "\\sce_sys\\livearea";
			string patchRetail = outputPatchDir + "\\sce_sys\\retail";
			string patchRetailLivearea = outputPatchDir + "\\sce_sys\\retail\\livearea";
			string patchClearsign = outputPatchDir + "\\sce_sys\\clearsign";
			string patchKeystone = outputPatchDir + "\\sce_sys\\keystone";
			if (Directory.Exists(patchDir))
			{
				if (!Directory.Exists(outputPatchDir))
				{
					try { Directory.CreateDirectory(outputPatchDir); }
					catch (Exception e) { MessageBox.Show("Could not create output directory " + outputPatchDir + ": " + e.Message); }
				}
				else
				{
					DeleteDirectory(outputPatchDir, true);
					try { Directory.CreateDirectory(outputPatchDir); }
					catch (Exception e) { MessageBox.Show("Could not create output directory " + outputPatchDir + ": " + e.Message); }
				}

				window.SetDecodingText(titleID + "_patch");
				window.SetDecodingPhase("Decrypting PFS");

                DecryptPFS(patchDir, outputPatchDir, klic);

				files = Directory.GetFiles(outputPatchDir, "*", SearchOption.AllDirectories);
				foreach (string SELF in files)
				{
					if (!IsSELF(SELF))
						continue;

					window.SetDecodingPhase("Decrypting " + Path.GetFileName(SELF));
					string tmpELF = SELF + ".elf";
					UnSELF(SELF, tmpELF, klic);

					ReadSELFHeader(SELF, out byte[] PIH, out byte[] NPDRM, out byte[] bootParams);
					DeleteFile(SELF);   //Delete old eboot

					MakeFSELF(tmpELF, SELF, compressCommand);
					DeleteFile(tmpELF);

					WriteSELFHeader(SELF, PIH, NPDRM, bootParams);
				}

				window.SetDecodingPhase("Patching param.sfo");
				PatchParamSFO(patchParamsfo);

				// Use retail livearea for game demos
				if (Directory.Exists(patchRetailLivearea))
				{
					DeleteDirectory(patchLivearea, true);
					Directory.Move(patchRetailLivearea, patchLivearea);
					DeleteDirectory(patchRetail, true);
				}

				DeleteFile(patchClearsign);
				DeleteFile(patchKeystone);

				if (o.MergePatch)
				{
					string tmp = outputBaseDir + "\\" + titleID;
					if (outputDir == tmp)
						MoveDirectory(outputPatchDir, outputDir);
					else
					{
						try
						{
							Directory.Move(outputDir, tmp);
							MoveDirectory(outputPatchDir, tmp);
							Directory.Move(tmp, outputDir);
						}
						catch (Exception e) { MessageBox.Show("Could not merge patch: " + e.Message); }
					}
					if (IsDirectoryEmpty(outputPatchBaseDir)) DeleteDirectory(outputPatchBaseDir);
				}
			}

			#endregion

			#region addcont

			if (window.Canceled) break;

			// Addcont Paths
			string addcontBaseDir = Directory.GetParent(inputDirTrimmed).ToString().TrimEnd(Path.DirectorySeparatorChar) + "\\addcont\\" + titleID;
			string outputAddcontBaseDirSuffix = "\\addcont\\";
			if (o.UseRePatch) outputAddcontBaseDirSuffix = "\\reAddcont\\";   //Use rePatch naming scheme 
			string outputAddcontBaseDir = Path.GetFullPath(o.OutputDir.TrimEnd(Path.DirectorySeparatorChar) + outputAddcontBaseDirSuffix + titleID);
			if (o.AddSuffix) outputAddcontBaseDir += "_dec";   //Check if suffix is needed

			string[] dirs = null;
			if (Directory.Exists(addcontBaseDir))
				dirs = Directory.GetDirectories(addcontBaseDir, "*", SearchOption.TopDirectoryOnly);

			if (dirs != null && dirs.Length > 0)
			{
				foreach (string directory in dirs)
				{
					string addcontName = Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar));
					string addcontDir = addcontBaseDir + "\\" + addcontName;
					string addcontWorkbin = addcontDir + "\\sce_sys\\package\\work.bin";
					string outputAddcontDir = outputAddcontBaseDir + "\\" + addcontName;
					string addcontParamsfo = outputAddcontDir + "\\sce_sys\\param.sfo";
					string addcontClearsign = outputAddcontDir + "\\sce_sys\\clearsign";
					string addcontKeystone = outputAddcontDir + "\\sce_sys\\keystone";

					if (!Directory.Exists(outputAddcontDir))
					{
						try { Directory.CreateDirectory(outputAddcontDir); }
						catch (Exception e) { MessageBox.Show("Could not create output directory " + outputAddcontDir + ": " + e.Message); }
					}
					else
					{
						DeleteDirectory(outputAddcontDir, true);
						try { Directory.CreateDirectory(outputAddcontDir); }
						catch (Exception e) { MessageBox.Show("Could not create output directory " + outputAddcontDir + ": " + e.Message); }
					}

					window.SetDecodingText(titleID + "_addcont");
					window.SetDecodingPhase("Decrypting PFS");

					string addcontKlic = GetKLicensee(addcontWorkbin);
                    DecryptPFS(addcontDir, outputAddcontDir, addcontKlic);

					files = Directory.GetFiles(outputAddcontDir, "*", SearchOption.AllDirectories);
					foreach (string SELF in files)
					{
						if (!IsSELF(SELF))
							continue;

						window.SetDecodingPhase("Decrypting " + Path.GetFileName(SELF));
						string tmpELF = SELF + ".elf";
						UnSELF(SELF, tmpELF, addcontKlic);

						ReadSELFHeader(SELF, out byte[] PIH, out byte[] NPDRM, out byte[] bootParams);
						DeleteFile(SELF);   //Delete old eboot

						MakeFSELF(tmpELF, SELF, compressCommand);
						DeleteFile(tmpELF);

						WriteSELFHeader(SELF, PIH, NPDRM, bootParams);
					}

					window.SetDecodingPhase("Patching param.sfo");
					PatchParamSFO(addcontParamsfo);

					DeleteFile(addcontClearsign);
					DeleteFile(addcontKeystone);
				}
			}

			#endregion

			if (o.MakeVPK)
			{
				window.SetDecodingPhase("Making VPK");
				DeleteFile(outputDir + ".vpk");
                MakeVPK(outputDir, outputDir + ".vpk");
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