using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace Blobs.Levels
{
	/// <summary>
	/// First level of gameplay with two small enemies
	/// </summary>
	public class Level1 : WalledLevel
	{
		public Level1(EntitySystem entitySystem, ScreenSpace screen, InputCommands input,
			ContentLoader content)
			: base(entitySystem, screen, input, content)
		{
			Color = Color.Yellow;
		}

		protected override void PositionPlayer()
		{
			Player.Center = new Point(0.4f, 0.25f);
			Player.Velocity = new Point(0.8f, 0.0f);
			Player.SetDamping(0.85f, 0.06f);
		}

		protected override void PositionCamera()
		{
			camera.LookAt = Point.Half;
			camera.Zoom = 0.9f;
		}

		protected override void AddPlatforms()
		{
			AddPlatform(new Rectangle(0.4f, 0.4f, 0.5f, 0.05f), 135, Color);
			AddPlatform(new Rectangle(0.875f, 0.5f, 0.15f, 0.05f), 0, Color);
			AddPlatform(new Rectangle(0.0f, 0.5f, 0.15f, 0.05f), 0, Color);
		}

		protected override void AddEnemies()
		{
			AddEnemy(new Point(0.05f, 0.32f), 0.04f);
			AddEnemy(new Point(0.9f, 0.35f), 0.04f);
		}

		protected override void AddText()
		{
			//TODO: VectorText
			//text1 = new VectorText(vectorData, new Point(0.15f, 0.3f), 0.05f) { Text = "Level 1" };
			//text2 = new VectorText(vectorData, new Point(0.15f, 0.4f), 0.05f) { Text = "Drag to shoot" };
			//Renderer.Add(text1);
			//Renderer.Add(text2);
		}

		//private VectorText text1;
		//private VectorText text2;

		protected override void SetupEvents() {}

		public override void Dispose()
		{
			base.Dispose();
			//text1.Dispose();
			//text2.Dispose();
		}
	}
}