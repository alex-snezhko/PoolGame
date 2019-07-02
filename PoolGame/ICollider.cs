namespace PoolGame
{
	interface ICollider
	{
		// returns float in [0-1] indicating how much of path objects completed when collided, or null if no collision
		float CollisionDistance(Ball ball);
		// handles collision of this collider with ball in question
		void Collide(Ball ball);
	}
}
