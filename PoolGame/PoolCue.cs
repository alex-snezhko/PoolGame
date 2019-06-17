using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace PoolGame
{
	class ShotEventArgs : EventArgs
	{
		public readonly int Power;

		public ShotEventArgs(int power)
		{
			Power = power;
		}
	}

	class PoolCue
	{
		CueBall cueBall;
		Vector2 posRelativeToBall = Vector2.Zero;
		bool charging;

		const float MAX_DIST = 0.25f;

		public event EventHandler<ShotEventArgs> ShotCharging;
		public event EventHandler<EventArgs> ShotCompleted;

		public PoolCue(CueBall cb)
		{
			cueBall = cb;
		}

		// changes position of cue based on mouse location
		public void ChangePos(object sender, MouseEventArgs e)
		{
			Vector2 mouseTablePoint = MainForm.FormToTablePoint(e.Location);
			posRelativeToBall = mouseTablePoint - cueBall.Position;

			// pool cue striker can only be a maximum of MAX_DIST meters away from the cue ball
			float dist = posRelativeToBall.Length();
			if(dist > MAX_DIST)
			{
				posRelativeToBall *= MAX_DIST / dist;
			}

			if(e.Button == MouseButtons.Left)
			{
				int power = (int)Math.Round(dist / MAX_DIST);
				ShotCharging(this, new ShotEventArgs(power));
			}
		}

		// shoots cue ball based on distance of cue striker relative to ball
		public void Shoot(object sender, MouseEventArgs e)
		{
			if (posRelativeToBall == Vector2.Zero)
			{
				return;
			}

			// LMB released; max speed is 11 m/s (when cue is MAX_DIST away from cue ball)
			if (e.Button == MouseButtons.Left)
			{
				cueBall.Shoot(11 / MAX_DIST * -posRelativeToBall);
				ShotCompleted(this, EventArgs.Empty);
			}
		}
	}
}
