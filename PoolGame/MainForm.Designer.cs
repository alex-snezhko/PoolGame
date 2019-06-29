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
			this.label1 = new System.Windows.Forms.Label();
			this.imgPowerBar = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.imgTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imgPowerBar)).BeginInit();
			this.SuspendLayout();
			// 
			// imgTable
			// 
			this.imgTable.Enabled = false;
			this.imgTable.Image = ((System.Drawing.Image)(resources.GetObject("imgTable.Image")));
			this.imgTable.Location = new System.Drawing.Point(30, 30);
			this.imgTable.Name = "imgTable";
			this.imgTable.Size = new System.Drawing.Size(466, 866);
			this.imgTable.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.imgTable.TabIndex = 0;
			this.imgTable.TabStop = false;
			// 
			// timer
			// 
			this.timer.Interval = 10;
			this.timer.Tick += new System.EventHandler(this.Timer_Tick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(526, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Shot Power:";
			// 
			// imgPowerBar
			// 
			this.imgPowerBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imgPowerBar.Enabled = false;
			this.imgPowerBar.Location = new System.Drawing.Point(529, 57);
			this.imgPowerBar.Name = "imgPowerBar";
			this.imgPowerBar.Size = new System.Drawing.Size(60, 500);
			this.imgPowerBar.TabIndex = 2;
			this.imgPowerBar.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 961);
			this.Controls.Add(this.imgPowerBar);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.imgTable);
			this.Name = "MainForm";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.imgTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imgPowerBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox imgTable;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox imgPowerBar;
	}
}

