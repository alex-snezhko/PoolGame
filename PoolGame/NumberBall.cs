using System;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;
using static PoolGame.GameManager;

namespace PoolGame
{
	class NumberBall : Ball
	{
		public readonly int number;
		public readonly bool solid; // true if solid, false if striped
		protected override float Mass { get => 0.16f; } // in kg

		public NumberBall(int num, PictureBox pic) : base(pic)
		{
			number = num;

			// finds appropriate starting position and solid/striped based on number on the ball
			solid = num <= 7 ? true : false;
			float eightBallX = TABLE_WIDTH / 2;
			float eightBallY = 3 * TABLE_HEIGHT / 4;
			float ballSeparation = 2 * RADIUS + 0.001f;
			float cos60Deg = (float)Math.Cos(Math.PI / 3);
			float sin60Deg = (float)Math.Sin(Math.PI / 3);
			switch (num)
			{
				case 1:
					Position = new Vector2(eightBallX, eightBallY - 2 * ballSeparation * sin60Deg);
					break;
				case 2:
					Position = new Vector2(eightBallX + ballSeparation * cos60Deg, eightBallY - ballSeparation * sin60Deg);
					break;
				case 3:
					Position = new Vector2(eightBallX - ballSeparation, eightBallY);
					break;
				case 4:
					Position = new Vector2(eightBallX - ballSeparation - ballSeparation * cos60Deg, eightBallY + ballSeparation * sin60Deg);
					break;
				case 5:
					Position = new Vector2(eightBallX, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 6:
					Position = new Vector2(eightBallX + 2 * ballSeparation, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 7:
					Position = new Vector2(eightBallX + ballSeparation * cos60Deg, eightBallY + ballSeparation * sin60Deg);
					break;
				case 8:
					Position = new Vector2(eightBallX, eightBallY);
					break;
				case 9:
					Position = new Vector2(eightBallX - ballSeparation, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 10:
					Position = new Vector2(eightBallX - ballSeparation * cos60Deg, eightBallY + ballSeparation * sin60Deg);
					break;
				case 11:
					Position = new Vector2(eightBallX + ballSeparation, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 12:
					Position = new Vector2(eightBallX + ballSeparation, eightBallY);
					break;
				case 13:
					Position = new Vector2(eightBallX - 2 * ballSeparation, eightBallY + 2 * ballSeparation * sin60Deg);
					break;
				case 14:
					Position = new Vector2(eightBallX - ballSeparation * cos60Deg, eightBallY - ballSeparation * sin60Deg);
					break;
				case 15:
					Position = new Vector2(eightBallX + ballSeparation + ballSeparation * cos60Deg, eightBallY + ballSeparation * sin60Deg);
					break;
			}

			MovePictureBox();
		}

		// display images of pocketed balls in section of GUI
		public override void Pocket()
		{
			// restarts game if 8-ball pocketed but other balls have not been
			if(number == 8 && BallsPocketed != 14)
			{
				//RestartGame();
			}

			ballImage.Size = new Size(35, 35);

			// coordinates of top-left point of grid where ball images will be placed
			const int BEGIN_X = START_X + 2 * BORDER_WIDTH + PLAYAREA_W_PIX + 30;
			const int BEGIN_Y = 600;

			// balls will be displayed in a grid with 3 columns;
			int row = BallsPocketed / 3;
			int column = BallsPocketed % 3;
			ballImage.Location = new Point(BEGIN_X + 40 * column, BEGIN_Y + 40 * row);

			ballImage.BackColor = Color.FromKnownColor(KnownColor.Control);
			BallsPocketed++;

			base.Pocket();
		}
	}
}
