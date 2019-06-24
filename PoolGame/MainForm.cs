using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using static PoolGame.GameManager;

namespace PoolGame
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			BeginGame(this, timer.Interval);
			this.imgTable.SendToBack();

			this.MouseMove += (sender, e) =>
			{
				if (!InPlay)
				{
					Cue.ChangePos(sender, e);
				}
			};
			this.MouseUp += (sender, e) =>
			{
				if (!InPlay)
				{
					Cue.Shoot(sender, e);
				}
			};

			Cue.ShotCharging += (sender, e) =>
			{
				this.pbShotPower.Enabled = true;
				//this.pbShotPower.Value = e.Power;
			};
			Cue.ShotCompleted += (sender, e) =>
			{
				this.pbShotPower.Enabled = false;

				InPlay = true;
				MoveBalls();			

				// resets timer
				this.timer.Stop();
				this.timer.Start();
			};
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (InPlay)
			{
				MoveBalls();
			}
		}
	}
}
