namespace PSVitaNPDRMDecryptor
{
	partial class OptionsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.chkCompressELFs = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.lblOutputDir = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.lblEnumerationStatus = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.chkAddSuffix = new System.Windows.Forms.CheckBox();
            this.toolTipAdd = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipRemove = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipStart = new System.Windows.Forms.ToolTip(this.components);
            this.chkVPK = new System.Windows.Forms.CheckBox();
            this.chkUseTitleID = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkCompressELFs
            // 
            this.chkCompressELFs.AutoSize = true;
            this.chkCompressELFs.Checked = true;
            this.chkCompressELFs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompressELFs.Location = new System.Drawing.Point(7, 186);
            this.chkCompressELFs.Name = "chkCompressELFs";
            this.chkCompressELFs.Size = new System.Drawing.Size(99, 17);
            this.chkCompressELFs.TabIndex = 4;
            this.chkCompressELFs.Text = "Compress ELFs";
            this.chkCompressELFs.UseVisualStyleBackColor = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(360, 157);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(32, 20);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHelp.Location = new System.Drawing.Point(3, 115);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(50, 23);
            this.btnHelp.TabIndex = 0;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDir.Location = new System.Drawing.Point(91, 157);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(263, 20);
            this.txtOutputDir.TabIndex = 5;
            this.txtOutputDir.Text = "./output";
            // 
            // lblOutputDir
            // 
            this.lblOutputDir.Location = new System.Drawing.Point(4, 157);
            this.lblOutputDir.Margin = new System.Windows.Forms.Padding(3);
            this.lblOutputDir.Name = "lblOutputDir";
            this.lblOutputDir.Size = new System.Drawing.Size(90, 20);
            this.lblOutputDir.TabIndex = 4;
            this.lblOutputDir.Text = "Output directory:";
            this.lblOutputDir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listBox1
            // 
            this.listBox1.AllowDrop = true;
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(392, 150);
            this.listBox1.TabIndex = 0;
            this.listBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox1_DragDrop);
            this.listBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox1_DragEnter);
            // 
            // lblEnumerationStatus
            // 
            this.lblEnumerationStatus.AutoSize = true;
            this.lblEnumerationStatus.Location = new System.Drawing.Point(3, 141);
            this.lblEnumerationStatus.Name = "lblEnumerationStatus";
            this.lblEnumerationStatus.Size = new System.Drawing.Size(10, 13);
            this.lblEnumerationStatus.TabIndex = 3;
            this.lblEnumerationStatus.Text = "\t ";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnAdd);
            this.flowLayoutPanel1.Controls.Add(this.btnRemove);
            this.flowLayoutPanel1.Controls.Add(this.btnHelp);
            this.flowLayoutPanel1.Controls.Add(this.lblEnumerationStatus);
            this.flowLayoutPanel1.Controls.Add(this.btnOkay);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(393, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(56, 226);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAdd.BackgroundImage = global::PSVitaNPDRMDecryptor.Properties.Resources.Plus_x42;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(50, 50);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRemove.BackgroundImage = global::PSVitaNPDRMDecryptor.Properties.Resources.Cross_x42;
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRemove.Location = new System.Drawing.Point(3, 59);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(50, 50);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOkay.BackgroundImage = global::PSVitaNPDRMDecryptor.Properties.Resources.Arrow_x42;
            this.btnOkay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOkay.Location = new System.Drawing.Point(3, 173);
            this.btnOkay.Margin = new System.Windows.Forms.Padding(3, 19, 3, 3);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(50, 50);
            this.btnOkay.TabIndex = 3;
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // chkAddSuffix
            // 
            this.chkAddSuffix.AutoSize = true;
            this.chkAddSuffix.Location = new System.Drawing.Point(7, 209);
            this.chkAddSuffix.Name = "chkAddSuffix";
            this.chkAddSuffix.Size = new System.Drawing.Size(109, 17);
            this.chkAddSuffix.TabIndex = 7;
            this.chkAddSuffix.Text = "Add \"_dec\" suffix";
            this.chkAddSuffix.UseVisualStyleBackColor = false;
            // 
            // toolTipAdd
            // 
            this.toolTipAdd.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // chkVPK
            // 
            this.chkVPK.AutoSize = true;
            this.chkVPK.Location = new System.Drawing.Point(110, 186);
            this.chkVPK.Name = "chkVPK";
            this.chkVPK.Size = new System.Drawing.Size(77, 17);
            this.chkVPK.TabIndex = 8;
            this.chkVPK.Text = "Make VPK";
            this.chkVPK.UseVisualStyleBackColor = false;
            // 
            // chkUseTitleID
            // 
            this.chkUseTitleID.AutoSize = true;
            this.chkUseTitleID.Location = new System.Drawing.Point(193, 186);
            this.chkUseTitleID.Name = "chkUseTitleID";
            this.chkUseTitleID.Size = new System.Drawing.Size(155, 17);
            this.chkUseTitleID.TabIndex = 9;
            this.chkUseTitleID.Text = "Use TitleID as output name";
            this.chkUseTitleID.UseVisualStyleBackColor = false;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 231);
            this.Controls.Add(this.chkUseTitleID);
            this.Controls.Add(this.chkVPK);
            this.Controls.Add(this.chkAddSuffix);
            this.Controls.Add(this.chkCompressELFs);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.lblOutputDir);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OptionsForm";
            this.Text = "PSVita NPDRM Decryptor";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnOkay;
		private System.Windows.Forms.TextBox txtOutputDir;
		private System.Windows.Forms.Label lblOutputDir;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.Label lblEnumerationStatus;
		private System.Windows.Forms.CheckBox chkCompressELFs;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox chkAddSuffix;
        private System.Windows.Forms.ToolTip toolTipAdd;
        private System.Windows.Forms.ToolTip toolTipRemove;
        private System.Windows.Forms.ToolTip toolTipStart;
        private System.Windows.Forms.CheckBox chkVPK;
        private System.Windows.Forms.CheckBox chkUseTitleID;
    }
}