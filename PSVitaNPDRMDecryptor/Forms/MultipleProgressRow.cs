using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSVitaNPDRMDecryptor; 
public partial class MultipleProgressRow : UserControl {
	public float Ratio { get; private set; }

	public MultipleProgressRow() {
		InitializeComponent();
	}

	public MultipleProgressRow(string text) {
		InitializeComponent();
		label1.Text = text;
		pnlProgress.Paint += pnlProgress_Paint;
	}

	private void pnlProgress_Paint(object sender, PaintEventArgs e) {
		float width = pnlProgress.Width * Math.Max(0, Math.Min(Ratio, 1));
		e.Graphics.FillRectangle(SystemBrushes.Highlight, 0, 0, width, pnlProgress.Height);
	}

	public void ShowProgress() {
		if (this.InvokeRequired) {
			this.BeginInvoke(new Action(ShowProgress));
		} else {
			pnlProgress.Visible = true;
		}
	}

	public void Remove() {
		if (this.Parent != null) {
			if (this.Parent.InvokeRequired) {
				this.Parent.BeginInvoke(new Action(Remove));
			} else {
				this.Parent.Controls.Remove(this);
			}
		}
	}

	public void Update(float ratio) {
		Ratio = ratio;
		pnlProgress.Invalidate();
	}
}
