using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace Dark
{
	public class SceneIntro : Scene
	{
		public SceneIntro(Game game, InputCommands input, ScreenSpace screenSpace)
			: base(game, input, screenSpace) {}

		public override void Load()
		{
			logo = new Sprite(ContentLoader.Load<Image>("DeltaEngineLogo"),
				new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			rand = new Random();
		}

		private Sprite logo;

		private Random rand;

		public override void Release()
		{
			logo.IsActive = false;
		}

		public override void Update()
		{
			if (step)
				direction = new Point((float)(rand.NextDouble() * 0.01f), (float)rand.NextDouble() * 0.01f);
			else
				direction = -direction;

			step = !step;
			logo.Center = new Point(direction.X + logo.Center.X,
				direction.Y + logo.Center.Y);
			elapsedTime += Time.Current.Delta;
			if (elapsedTime >= IntroSeconds)
				game.SetScene(Scenes.Level);
		}

		private Point direction;
		private bool step;
		private float elapsedTime;
		private const float IntroSeconds = 3.0f;
	}
}