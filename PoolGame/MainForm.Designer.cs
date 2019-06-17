namespace PoolGame
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.imgTable = new System.Windows.Forms.PictureBox();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.pbShotPower = new System.Windows.Forms.ProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.imgTable)).BeginInit();
			this.SuspendLayout();
			// 
			// imgTable
			// 
			this.imgTable.Image = ((System.Drawing.Image)(resources.GetObject("imgTable.Image")));
			this.imgTable.Location = new System.Drawing.Point(30, 30);
			this.imgTable.Name = "imgTable";
			this.imgTable.Size = new System.Drawing.Size(444, 834);
			this.imgTable.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.imgTable.TabIndex = 0;
			this.imgTable.TabStop = false;
			// 
			// timer
			// 
			this.timer.Interval = 10;
			this.timer.Tick += new System.EventHandler(this.Timer_Tick);
			// 
			// pbShotPower
			// 
			this.pbShotPower.Location = new System.Drawing.Point(483, 256);
			this.pbShotPower.Name = "pbShotPower";
			this.pbShotPower.Size = new System.Drawing.Size(100, 23);
			this.pbShotPower.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(595, 859);
			this.Controls.Add(this.pbShotPower);
			this.Controls.Add(this.imgTable);
			this.Name = "MainForm";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.imgTable)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox imgTable;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ProgressBar pbShotPower;
	}
}

