using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blobs.Levels
{
	/// <summary>
	/// Second level of gameplay with red enemies
	/// </summary>
	public class Level2 : WalledLevel
	{
		public Level2(ScreenSpace screen, InputCommands input)
			: base(screen, input)
		{
			Color = Color.LightBlue;
		}

		protected override void PositionPlayer()
		{
			Player.Center = new Point(0.4f, 0.25f);
			Player.Velocity = new Point(0.8f, 0.0f);
			Player.SetDamping(0.7f, 0.1f);
		}

		protected override void PositionCamera()
		{
			camera.LookAt = Point.Half;
			camera.Zoom = 0.5f;
		}

		protected override void AddPlatforms()
		{
			AddPlatform(new Rectangle(0.4f, 0.4f, 0.5f, 0.05f), 135, Color);
			AddPlatform(new Rectangle(0.875f, 0.5f, 0.15f, 0.05f), 0, Color);
			AddPlatform(new Rectangle(0.0f, 0.5f, 0.15f, 0.05f), 0, Color);
		}

		protected override void AddEnemies()
		{
			AddEnemy(new Point(0.05f, 0.32f), 0.03f);
			AddEnemy(new Point(0.9f, 0.35f), 0.055f);
			AddEnemy(new Point(1.1f, 0.32f), 0.065f);
		}

		protected override void SetText()
		{
			text.IsActive = true;
			text.Text = "Eat Greens... Get Bigger... Reds turn green";
			text.SetPosition(new Point(0.2f, 0.1f));
		}

		protected override void SetupEvents() {}
	}
}