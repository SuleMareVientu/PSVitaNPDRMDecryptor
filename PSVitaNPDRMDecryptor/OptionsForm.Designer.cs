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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.chkCompressELFs = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.lblOutputDir = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lblEnumerationStatus = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chkAddSuffix = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkCompressELFs
            // 
            this.chkCompressELFs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCompressELFs.AutoSize = true;
            this.chkCompressELFs.Checked = true;
            this.chkCompressELFs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompressELFs.Location = new System.Drawing.Point(117, 230);
            this.chkCompressELFs.Margin = new System.Windows.Forms.Padding(4);
            this.chkCompressELFs.Name = "chkCompressELFs";
            this.chkCompressELFs.Size = new System.Drawing.Size(125, 20);
            this.chkCompressELFs.TabIndex = 4;
            this.chkCompressELFs.Text = "Compress ELFs";
            this.chkCompressELFs.UseVisualStyleBackColor = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(477, 192);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(43, 25);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHelp.Location = new System.Drawing.Point(9, 224);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(4);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(100, 28);
            this.btnHelp.TabIndex = 0;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOkay.Location = new System.Drawing.Point(4, 186);
            this.btnOkay.Margin = new System.Windows.Forms.Padding(4, 98, 4, 4);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(99, 64);
            this.btnOkay.TabIndex = 3;
            this.btnOkay.Text = "Start";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDir.Location = new System.Drawing.Point(117, 192);
            this.txtOutputDir.Margin = new System.Windows.Forms.Padding(4);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(351, 22);
            this.txtOutputDir.TabIndex = 5;
            this.txtOutputDir.Text = "./output";
            // 
            // lblOutputDir
            // 
            this.lblOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOutputDir.Location = new System.Drawing.Point(5, 192);
            this.lblOutputDir.Margin = new System.Windows.Forms.Padding(4);
            this.lblOutputDir.Name = "lblOutputDir";
            this.lblOutputDir.Size = new System.Drawing.Size(120, 25);
            this.lblOutputDir.TabIndex = 4;
            this.lblOutputDir.Text = "Output directory:";
            this.lblOutputDir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listBox1
            // 
            this.listBox1.AllowDrop = true;
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(528, 184);
            this.listBox1.TabIndex = 0;
            this.listBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox1_DragDrop);
            this.listBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox1_DragEnter);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(4, 4);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(99, 28);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add Folders";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(4, 40);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(99, 28);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lblEnumerationStatus
            // 
            this.lblEnumerationStatus.AutoSize = true;
            this.lblEnumerationStatus.Location = new System.Drawing.Point(4, 72);
            this.lblEnumerationStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEnumerationStatus.Name = "lblEnumerationStatus";
            this.lblEnumerationStatus.Size = new System.Drawing.Size(10, 16);
            this.lblEnumerationStatus.TabIndex = 3;
            this.lblEnumerationStatus.Text = "\t ";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnAdd);
            this.flowLayoutPanel1.Controls.Add(this.btnRemove);
            this.flowLayoutPanel1.Controls.Add(this.lblEnumerationStatus);
            this.flowLayoutPanel1.Controls.Add(this.btnOkay);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(528, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(107, 258);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // chkAddSuffix
            // 
            this.chkAddSuffix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAddSuffix.AutoSize = true;
            this.chkAddSuffix.Location = new System.Drawing.Point(257, 230);
            this.chkAddSuffix.Margin = new System.Windows.Forms.Padding(4);
            this.chkAddSuffix.Name = "chkAddSuffix";
            this.chkAddSuffix.Size = new System.Drawing.Size(129, 20);
            this.chkAddSuffix.TabIndex = 7;
            this.chkAddSuffix.Text = "Add \"_dec\" suffix";
            this.chkAddSuffix.UseVisualStyleBackColor = false;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 258);
            this.Controls.Add(this.chkAddSuffix);
            this.Controls.Add(this.chkCompressELFs);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.lblOutputDir);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
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
    }
}