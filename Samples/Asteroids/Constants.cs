namespace Asteroids
{
	/// <summary>
	/// Collection of constants used during the game Asteroids
	/// </summary>
	public class Constants
	{
		public const int PlayerMaxHp = 100;
		public const float PlayerAcceleration = 1;
		public const float PlayerDecelerationFactor = 0.97f;
		public const float PlayerTurnSpeed = 160;
		public const float MaximumObjectVelocity = .5f;
		public const float ProjectileVelocity = .5f;
		public const float PlayerCadance = 0.003f;
		public const float playerDecelFactor = 0.7f;
		public const int maximumAsteroids = 10;

		public enum RenderLayer
		{
			Background,
			Rockets,
			Player,
			Asteroids,
			UserInterface
		}
	}
}
