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
		readonly Graphics powerBarGfx;

		public MainForm()
		{
			InitializeComponent();
			powerBarGfx = this.imgPowerBar.CreateGraphics();
			this.Paint += (sender, e) =>
			{
				DrawShotPowerBar(0f);
			};

			BeginGame(this, this.timer.Interval);
			this.imgTable.SendToBack();

			this.MouseMove += (sender, e) =>
			{
				// moves crosshair image on GUI if cue ball is scratched to indicate where it will be placed 
				if (!BallsMoving && Scratched)
				{
					MoveCrossHair(e.Location);
				}
			};

			this.MouseMove += MoveAndChargeCue;
			this.MouseDown += MoveAndChargeCue;
			void MoveAndChargeCue(object sender, MouseEventArgs e)
			{
				if (!BallsMoving && !Scratched)
				{
					// adjusts angle of attack on cue ball
					Cue.ChangePos(e.Location);
					if (e.Button == MouseButtons.Left)
					{
						float power = Cue.ChargeShot();
						DrawShotPowerBar(power);
					}
				}
			}

			this.MouseUp += (sender, e) =>
			{
				if (e.Button == MouseButtons.Left && !BallsMoving)
				{
					if (Scratched)
					{
						PlaceCueBall(e.Location);
					}
					else
					{
						Cue.Shoot(e.Location);

						DrawShotPowerBar(0f);
						BallsMoving = true;
						MoveBalls();

						// resets timer
						this.timer.Stop();
						this.timer.Start();
					}
				}
			};	
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (BallsMoving)
			{
				this.timer.Stop();
				MoveBalls();
				this.timer.Start();
			}
		}
		
		// draws bar of variable side on GUI indicating strength of charging shot
		private void DrawShotPowerBar(float val)
		{
			if(val > 1f)
			{
				return;
			}

			// erase previously drawn bars
			powerBarGfx.Clear(Color.FromKnownColor(KnownColor.Control));

			// draws power component
			SolidBrush redBrush = new SolidBrush(Color.Red);		
			int height = (int)Math.Round(this.imgPowerBar.Height * val);
			Rectangle power = new Rectangle(0, this.imgPowerBar.Height - height, this.imgPowerBar.Width, height);
			powerBarGfx.FillRectangle(redBrush, power);
			redBrush.Dispose();
		}
	}
}
