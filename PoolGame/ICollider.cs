namespace PoolGame
{
	interface ICollider
	{
		// true -> re-adds this ball to list of balls that are moving to check for further collisions
		bool WillCollideWith(Ball ball);
		float DistanceToCollision(Ball ball);
		//bool CheckForCollision(ICollider other);
	}
}
