using System.Collections.Generic;

namespace PSVitaNPDRMDecryptor;
public class Options
{
	public IEnumerable<string> InputFolders { get; set; }
	public string OutputDir { get; set; }
	public bool CompressELFs { get; set; }
	public bool MergePatch { get; set; }
	public bool MakeVPK { get; set; }
	public bool LookForAddcont { get; set; }
	public bool UseTitleID { get; set; }
	public bool AddSuffix { get; set; }
	public bool UseRePatch { get; set; }
	public bool CopyHeadBin { get; set; }
	public bool ShowCMD { get; set; }
}
