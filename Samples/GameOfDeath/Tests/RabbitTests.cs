using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.ScreenSpaces;

namespace GameOfDeath.Tests
{
	public class RabbitTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void ShowSingleRabbit(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var rabbitImage = content.Load<Image>("Rabbit");
				CreateRabbitWith50Health(rabbitImage, Point.Half);
			});
		}

		[VisualTest]
		public void DamageSingleRabbitToHalfHealth(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var rabbitImage = content.Load<Image>("Rabbit");
				var rabbit = CreateRabbitWith50Health(rabbitImage, Point.Half);
				rabbit.DoDamage(25);
			});
		}

		private static Rabbit CreateRabbitWith50Health(Image rabbitImage, Point position)
		{
			var rabbit = new Rabbit(rabbitImage, position);
			rabbit.SetHealth(50);
			return rabbit;
		}

		[VisualTest]
		public void ShowManyRabbits(Type resolver)
		{
			Start(resolver, (ContentLoader content, ScreenSpace screen) =>
			{
				var rabbitImage = content.Load<Image>("Rabbit");
				var viewport = screen.Viewport;
				var size = RabbitGrid.CellSize;
				for (float x = viewport.Left + size.Width / 2; x <= viewport.Right; x += size.Width)
					for (float y = viewport.Top + size.Height / 2; y <= viewport.Bottom; y += size.Height)
						CreateRabbitWith50Health(rabbitImage, new Point(x, y));
			});
		}

		[VisualTest]
		public void SpawnDeadRabbitAtMousePosition(Type resolver)
		{
			Start(resolver, (ContentLoader content, InputCommands input) =>
			{
				var deadRabbitImage = content.Load<Image>("DeadRabbit");
				input.Add(MouseButton.Left,
					mouse =>
						new DeadRabbit(deadRabbitImage,
							Rectangle.FromCenter(mouse.Position,
								deadRabbitImage.PixelSize / Scoreboard.QuadraticFullscreenSize)));
				if (mockResolver == null)
					return;

				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Releasing);
				mockResolver.AdvanceTimeAndExecuteRunners(1);
			});
		}
	}
}