using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;
using static System.Windows.Forms.AxHost;

namespace PSVitaNPDRMDecryptor;
public partial class OptionsForm : Form
{
	private class NVPair
	{
		public string Name { get; set; }
		public object Value { get; set; }
		public NVPair(object value, string name)
		{
			this.Name = name;
			this.Value = value;
		}
	}

	public void SetFormState(bool state)
	{
		Program.form.btnAdd.Enabled = state;
		Program.form.btnRemove.Enabled = state;
		Program.form.btnBrowse.Enabled = state;
		Program.form.btnOkay.Enabled = state;
		Program.form.btnHelp.Enabled = state;

		Program.form.listBox1.Enabled = state;
		Program.form.txtOutputDir.Enabled = state;
		Program.form.chkCompressELFs.Enabled = state;
		Program.form.chkMergePatch.Enabled = state;
		Program.form.chkVPK.Enabled = state;
		Program.form.chkLookForAddcont.Enabled = state;
		Program.form.chkUseTitleID.Enabled = state;
		Program.form.chkAddSuffix.Enabled = state;
		Program.form.chkUseRePatch.Enabled = state;
		Program.form.chkCopyHeadBin.Enabled = state;
		Program.form.chkShowCMD.Enabled = state;
		if (state)
		{
			chkLookForAddcont_CheckedChanged(null, null);
			chkVPK_CheckedChanged(null, null);
			chkCopyHeadBin_CheckedChanged(null, null);
		}
		return;
	}

	private HashSet<Task> runningTasks;

	public IEnumerable<Task> RunningTasks
	{
		get
		{
			return runningTasks;
		}
	}

	public OptionsForm()
	{
		InitializeComponent();
		toolTipAdd.SetToolTip(btnAdd, "Add Folders");

		toolTipRemove.SetToolTip(btnRemove, "Remove highlighted folders");

		toolTipStart.SetToolTip(btnOkay, "Run");

		toolTipCompressELFs.SetToolTip(chkCompressELFs, "Compress app executables");

		toolTipUseTitleID.SetToolTip(chkUseTitleID, "Use app's TitleID as output directory name");

		toolTipAddSuffix.SetToolTip(chkAddSuffix, "Add \"_dec\" as output directory name" + Environment.NewLine
			+ "(E.g. when the output dir is the same as the source dir)");

		toolTipLookForAddcont.SetToolTip(chkLookForAddcont, "Look for patches and additional content as described in the readme" + Environment.NewLine
			+ "(with \"appRoot\" being the root folder of the app folder:" + Environment.NewLine
			+ "\"appRoot\\patch\\appTitleID\"" + Environment.NewLine
			+ "\"appRoot\\addcont\\appTitleID\\AddcontID\")");

		toolTipMergePatch.SetToolTip(chkMergePatch, "Merge app and patch" + Environment.NewLine
			+ "(useful when not using rePatch / saving space)");
		toolTipVPK.SetToolTip(chkVPK, "Create a VPK package installable through VitaShell");

		toolTipUseRePatch.SetToolTip(chkUseRePatch, "Change output directories names to rePatch naming scheme" + Environment.NewLine
			+ "(E.g. \"patch\" -> \"rePatch\", \"addcont\" -> \"reAddcont\")");

		toolTipCopyHeadBin.SetToolTip(chkCopyHeadBin, "Copy original \"head.bin\" to output directory" + Environment.NewLine
			+ "(needed if one wants to use the \"Refresh LiveArea\" option in VitaShell" + Environment.NewLine
			+ "after copying decrypted content directly to \"ux0:app\" etc.," + Environment.NewLine
			+ "instead of installing apps manually through VitaShell)");

		toolTipShowCMD.SetToolTip(chkShowCMD, "Show command prompt during the decryption process");

		runningTasks = new HashSet<Task>();
	}

	public Options GetOptions()
	{
		List<string> filenames = new List<string>();
		foreach (object item in listBox1.Items)
		{
			filenames.Add(item.ToString());
		}
		return new Options
		{
			InputFolders = filenames,
			OutputDir = txtOutputDir.Text,
			CompressELFs = chkCompressELFs.Checked,
			MergePatch = chkMergePatch.Checked,
			MakeVPK = chkVPK.Checked,
			LookForAddcont = chkLookForAddcont.Checked,
			UseTitleID = chkUseTitleID.Checked,
			AddSuffix = chkAddSuffix.Checked,
			UseRePatch = chkUseRePatch.Checked,
			CopyHeadBin = chkCopyHeadBin.Checked,
			ShowCMD = chkShowCMD.Checked,
		};
	}

	private void btnAdd_Click(object sender, EventArgs e)
	{
		using (BetterFolderBrowser d = new BetterFolderBrowser())
		{
			SetFormState(false);
			d.Multiselect = true;
			//d.RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
			if (d.ShowDialog() == DialogResult.OK)
			{
				foreach (string filepath in d.SelectedFolders)
				{
					if (IsFolderValid(filepath))
						listBox1.Items.Add(filepath);
				}
			}
		}
		SetFormState(true);
	}

	private void btnRemove_Click(object sender, EventArgs e)
	{
		SortedSet<int> set = new SortedSet<int>();
		foreach (int index in listBox1.SelectedIndices)
		{
			set.Add(index);
		}
		foreach (int index in set.Reverse())
		{
			listBox1.Items.RemoveAt(index);
		}
	}

	private void listBox1_DragEnter(object sender, DragEventArgs e)
	{
		string[] data = e.Data.GetData("FileDrop") as string[];
		if (data != null && data.Length != 0)
		{
			e.Effect = DragDropEffects.Link;
		}
	}

	private void listBox1_DragDrop(object sender, DragEventArgs e)
	{
		e.Effect = DragDropEffects.None;

		string[] data = e.Data.GetData("FileDrop") as string[];
		if (data != null)
		{
			foreach (string filepath in data)
			{
				if (IsFolderValid(filepath))
					listBox1.Items.Add(filepath);
			}
		}
	}

	private void btnBrowse_Click(object sender, EventArgs e)
	{
		using (BetterFolderBrowser d = new BetterFolderBrowser())
		{
			SetFormState(false);
			//d.RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
			if (d.ShowDialog() == DialogResult.OK)
			{
				txtOutputDir.Text = d.SelectedPath;
			}
		}
		SetFormState(true);
	}

	private void btnHelp_Click(object sender, EventArgs e)
	{
		string msg = "• This tool works on encrypted PSVita games extracted with \"pkg2zip.exe\" or similar, it won't work on PKGs directly.\n\n" +
		"• Use the \"+\" button or drag & drop, select the output directory and \"Run\".\n\n" +
		"• Delete highlighted folders with the \"X\" button.";
		msg = msg.Replace("\n", Environment.NewLine);
		MessageBox.Show(msg, "Instructions", MessageBoxButtons.OK, MessageBoxIcon.Question);
	}

	private void btnOkay_Click(object sender, EventArgs e)
	{
		Options o = this.GetOptions();
		this.listBox1.Items.Clear();
		Task t = new Task(() => Program.Run(o));
		runningTasks.Add(t);
		UpdateTitle();
		t.Start();
		t.ContinueWith(x => {
			if (x.Exception != null)
			{
				Console.Error.WriteLine(x.Exception + ": " + x.Exception.Message);
				Console.Error.WriteLine(x.Exception.StackTrace);
			}
			runningTasks.Remove(x);
			UpdateTitle();
		});
	}

	private void UpdateTitle()
	{
		if (this.InvokeRequired)
		{
			this.BeginInvoke(new Action(UpdateTitle));
			return;
		}
		string text = this.Text + ":";
		text = text.Substring(0, text.IndexOf(':'));
		switch (runningTasks.Count)
		{
			case 0:
				break;
			case 1:
				text += ": batch running";
				break;
			default:
				text += ": " + runningTasks.Count + " batches running";
				break;
		}
		this.Text = text;
	}

	private bool IsFolderValid(string filepath)
	{
		if (!File.Exists(filepath) && Directory.Exists(filepath) && !listBox1.Items.Contains(filepath))
		{
			if (File.Exists(filepath.TrimEnd(Path.DirectorySeparatorChar) + "\\sce_sys\\package\\work.bin"))
				return true;
		}

		return false;
	}

	private void chkCopyHeadBin_CheckedChanged(object sender, EventArgs e)
	{
		if (chkCopyHeadBin.Checked)
		{
			chkVPK.Checked = false;
			chkVPK.Enabled = false;
		}
        else
			chkVPK.Enabled = true;
	}

	private void chkVPK_CheckedChanged(object sender, EventArgs e)
	{
		if (chkVPK.Checked)
		{
			chkCopyHeadBin.Checked = false;
			chkCopyHeadBin.Enabled = false;
		}
		else
			chkCopyHeadBin.Enabled = true;
	}

	private void chkLookForAddcont_CheckedChanged(object sender, EventArgs e)
	{
		if (!chkLookForAddcont.Checked)
		{
			chkMergePatch.Checked = false;
			chkMergePatch.Enabled = false;
			chkUseRePatch.Checked = false;
			chkUseRePatch.Enabled = false;
		}
		else
		{
			chkMergePatch.Enabled = true;
			chkUseRePatch.Enabled = true;
		}
	}
}