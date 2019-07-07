using System;
using System.Windows.Forms;
using System.Numerics;

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
			float eightBallX = GameManager.TABLE_WIDTH / 2;
			float eightBallY = 3 * GameManager.TABLE_HEIGHT / 4;
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

		public override void Pocket()
		{
			base.Pocket();
			GameManager.ActiveBalls.Remove(this);
			//GameManager.Colliders.Remove(this); 
			// TODO: work on this; make it work if cue ball gets in pocket as well
		}
	}
}
