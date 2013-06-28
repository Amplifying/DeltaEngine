using DeltaEngine.Datatypes;

namespace TinyPlatformer
{
	public class Constants
	{
		public const int TileSize = 32;
		public const int Meter = 32;
		public const float Gravity = 9.8f;// * 6.0f;
		public const int MaxXSpeed = 15;
		public const int MaxYSpeed = 60;
		public const float Accel = 0.5f;
		public const float Friction = 0.1667f;
		public const int Impulse = 750;
		public const float BlockSize = 0.0125f;
		public static Size ScreenGap = new Size(0.1f, 0.2f);
		public const float TimeStep = 1.0f / 60.0f;
	}

	public enum BlockType
	{
		None,
		Gold,
		GroundBrick,
		PlatformBrick,
		PlatformTop,
		LevelBorder
	}
	public enum EntityType
	{
		None,
		Player,
		Enemy
	}
}
