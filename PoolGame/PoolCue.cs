using System;
using System.Windows.Forms;
using System.Numerics;

namespace PoolGame
{
	class PoolCue
	{
		readonly CueBall cueBall;
		Vector2 posRelativeToBall = Vector2.Zero;

		const float MAX_DIST = 0.5f;

		public event EventHandler<float> ShotCharging;
		public event EventHandler ShotCompleted;

		public PoolCue(CueBall cb)
		{
			cueBall = cb;
		}

		// changes position of cue based on mouse location
		public void ChangePos(object sender, MouseEventArgs e)
		{
			Vector2 mouseTablePoint = GameManager.FormToTablePoint(e.Location);
			posRelativeToBall = mouseTablePoint - cueBall.Position;

			// pool cue striker can only be a maximum of MAX_DIST meters away from the cue ball
			float dist = posRelativeToBall.Length();
			if(dist > MAX_DIST)
			{
				posRelativeToBall *= MAX_DIST / dist;
			}

			if(e.Button == MouseButtons.Left)
			{
				float power = dist / MAX_DIST;
				ShotCharging(this, power);
			}
		}

		// shoots cue ball based on distance of cue striker relative to ball
		public void Shoot(object sender, MouseEventArgs e)
		{
			Vector2 mouseTablePoint = GameManager.FormToTablePoint(e.Location);
			posRelativeToBall = mouseTablePoint - cueBall.Position;

			// pool cue striker can only be a maximum of MAX_DIST meters away from the cue ball
			float dist = posRelativeToBall.Length();
			if (dist > MAX_DIST)
			{
				posRelativeToBall *= MAX_DIST / dist;
			}

			if (posRelativeToBall == Vector2.Zero)
			{
				return;
			}

			// complete shot; max speed is 5 m/s (when cue is MAX_DIST away from cue ball)
			cueBall.ApplyForce(5 / MAX_DIST * posRelativeToBall);
			ShotCompleted(this, EventArgs.Empty);
		}
	}
}
