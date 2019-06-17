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
using PoolGame.Properties;
using static PoolGame.GameManager;

namespace PoolGame
{
	public partial class MainForm : Form
	{
		PictureBox[] balls;

		const int START_X = 25, START_Y = 25, // table offset from top left of form
			TABLE_W_PIX = 444, TABLE_H_PIX = 834, BORDER_WIDTH = 15,
			PLAYAREA_W_PIX = TABLE_W_PIX - 2 * BORDER_WIDTH, PLAYAREA_H_PIX = TABLE_H_PIX - 2 * BORDER_WIDTH;

		public MainForm()
		{
			InitializeComponent();
			CreateBalls();
			BeginGame(timer.Interval);
			this.MouseMove += (sender, e) => { if (InPlay) Cue.ChangePos(sender, e); };
			this.MouseUp += (sender, e) => { if (InPlay) Cue.Shoot(sender, e); };

			Cue.ShotCharging += (sender, e) =>
			{
				this.pbShotPower.Enabled = true;
				this.pbShotPower.Value = e.Power;
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

		private void CreateBalls()
		{
			balls = new PictureBox[16];
			balls[0] = new PictureBox() { Image = Resources.cueball };
			balls[1] = new PictureBox() { Image = Resources._1ball };
			balls[2] = new PictureBox() { Image = Resources._2ball };
			balls[3] = new PictureBox() { Image = Resources._3ball };
			balls[4] = new PictureBox() { Image = Resources._4ball };
			balls[5] = new PictureBox() { Image = Resources._5ball };
			balls[6] = new PictureBox() { Image = Resources._6ball };
			balls[7] = new PictureBox() { Image = Resources._7ball };
			balls[8] = new PictureBox() { Image = Resources._8ball };
			balls[9] = new PictureBox() { Image = Resources._9ball };
			balls[10] = new PictureBox() { Image = Resources._10ball };
			balls[11] = new PictureBox() { Image = Resources._11ball };
			balls[12] = new PictureBox() { Image = Resources._12ball };
			balls[13] = new PictureBox() { Image = Resources._13ball };
			balls[14] = new PictureBox() { Image = Resources._14ball };
			balls[15] = new PictureBox() { Image = Resources._15ball };

			const int DIMENSION = (int)(2 * Ball.RADIUS / (PLAYAREA_W_PIX / TABLE_WIDTH));
			foreach(PictureBox b in balls)
			{
				b.Size = new Size(DIMENSION, DIMENSION);
			}
		}

		public static Vector2 FormToTablePoint(Point screenPoint)
		{
			Vector2 pointVector = new Vector2(screenPoint.X, screenPoint.Y);
			// ratio of one pixel length to one game unit (in meters)
			const float P2M_COEFF = TABLE_WIDTH / PLAYAREA_W_PIX;

			// mouse location relative to top-left of pool table playing area
			Vector2 fromTL = pointVector - new Vector2(START_X + BORDER_WIDTH, START_Y + BORDER_WIDTH);

			Vector2 tablePoint = new Vector2(fromTL.X * P2M_COEFF, (PLAYAREA_H_PIX - fromTL.Y) * P2M_COEFF);
			return tablePoint;
		}

		// converts pixel location on form to position on board in game units; (0,0) = bottom left of board
		public static Point TableToFormPoint(Vector2 location)
		{
			// ratio of one game unit (in meters) to pixel length
			const float M2P_COEFF = PLAYAREA_W_PIX / TABLE_WIDTH;
			// pixel equivalent of vector location (not accounting for table offset from form)
			int tableXPix = (int)(location.X * M2P_COEFF), tableYPix = (int)(location.Y * M2P_COEFF);

			Point screenPoint = new Point(tableXPix + START_X + BORDER_WIDTH, PLAYAREA_H_PIX - tableYPix + START_Y + BORDER_WIDTH);
			return screenPoint;
		}
	}
}
