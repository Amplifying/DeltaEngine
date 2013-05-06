using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace Blobs.Levels
{
	/// <summary>
	/// Intro page, inviting the player to click to start
	/// </summary>
	public class Intro : Level
	{
		public Intro(EntitySystem entitySystem, ScreenSpace screen, InputCommands input,
			ContentLoader content)
			: base(entitySystem, screen, input, content) {}

		protected override void PositionPlayer()
		{
			Player.Center = new Point(0.1f, 0.2f);
			Player.Velocity = new Point(0.2f, 0.0f);
		}

		protected override void PositionCamera()
		{
			camera.LookAt = Point.Half;
			camera.Zoom = 1.0f;
		}

		protected override void AddEnemies() {}

		protected override void AddPlatforms()
		{
			AddPlatform(new Rectangle(0.2f, 0.8f, 10.0f, 0.2f), 0.0f, Color.Green);
		}

		protected override void AddText()
		{
			//TODO: VectorText
			//Renderer.Add(
			//	text =
			//		new VectorText(vectorData, new Point(0.65f, 0.805f), 0.01f)
			//		{
			//			Text = "Click to start",
			//			Color = Color.Black
			//		});
		}

		//private VectorText text;

		protected override void SetupEvents()
		{
			command = input.Add(MouseButton.Left, trigger => PassLevel());
		}

		private Command command;

		public override void Dispose()
		{
			base.Dispose();
			//text.Dispose();
			input.Remove(command);
		}

		public override void Run()
		{
			base.Run();
			MoveCamera();
		}

		private void MoveCamera()
		{
			elapsed += Time.Current.Delta;

			if (elapsed > 3.0f)
				camera.LookAt = Point.Lerp(Point.Half, new Point(Player.Center.X, 0.75f),
					0.2f * (elapsed - 3.0f));

			if (elapsed > 8.0f)
				camera.Zoom += 0.02f;
		}
	}
}