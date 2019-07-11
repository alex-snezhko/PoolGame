using System.Windows.Forms;
using System.Numerics;

namespace PoolGame
{
	class CueBall : Ball
	{
		protected override float Mass { get => 0.17f; } // in kg

		public CueBall(PictureBox pic) : base(pic)
		{
			Position = new Vector2(0.6096f, 0.6096f);
			MovePictureBox();
		}

		// place cue ball on table after scratch
		public void PlaceBall(Vector2 location)
		{
			Position = location;
			// ball still considered moving after it is placed; negate this effect
			Velocity = Vector2.Zero;

			ballImage.Visible = true;
			MovePictureBox();
		}

		public override void Pocket()
		{
			base.Pocket();
			ballImage.Visible = false;
			GameManager.Scratched = true;
		}
	}
}
