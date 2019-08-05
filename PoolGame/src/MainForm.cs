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
			powerBarGfx = this.imgPowerBar.CreateGraphics();

			Init(this, this.timer.Interval);
			this.imgTable.SendToBack();

			AddFormEvents();
			DrawShotPowerBar(0f);

			// adds handlers for various winform events such as mouse clicks
			void AddFormEvents()
			{
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
				// readjusts position of cue
				void MoveAndChargeCue(object sender, MouseEventArgs e)
				{
					// adjusts angle of attack on cue ball if in shooting phase
					if (!BallsMoving && !Scratched)
					{
						Cue.ChangePos(e.Location);
						// draws shot power if mouse held down
						if (e.Button == MouseButtons.Left)
						{
							DrawShotPowerBar(Cue.ShotPower());
						}
					}
				}

				this.MouseUp += (sender, e) =>
				{
					if (e.Button == MouseButtons.Left && !BallsMoving)
					{
						// places cue ball onto table if left click while scratched
						if (Scratched)
						{
							PlaceCueBall(e.Location);
						}
						// shoots cue ball if left click while not scratched
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

		// graphics object for drawing shot power bar
		readonly Graphics powerBarGfx;

		// draws bar of variable size on GUI indicating strength of charging shot
		private void DrawShotPowerBar(float val)
		{
			if(val > 1f)
			{
				val = 1f;
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
