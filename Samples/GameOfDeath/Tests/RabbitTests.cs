using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;

namespace GameOfDeath.Tests
{
	class RabbitTests : TestStarter
	{
		[VisualTest]
		public void ShowSingleRabbit(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer) =>
			{
				var rabbitImage = content.Load<Image>("Rabbit");
				renderer.Add(CreateRabbitWith50Health(rabbitImage, Point.Half));
			});
		}

		[VisualTest]
		public void DamageSingleRabbitToHalfHealth(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer) =>
			{
				var rabbitImage = content.Load<Image>("Rabbit");
				var rabbit = CreateRabbitWith50Health(rabbitImage, Point.Half);
				rabbit.DoDamage(25);
				renderer.Add(rabbit);
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
			Start(resolver, (Content content, Renderer renderer, Randomizer random) =>
			{
				var rabbitImage = content.Load<Image>("Rabbit");
				var viewport = renderer.Screen.Viewport;
				var size = RabbitsGrid.CellSize;
				for (float x = viewport.Left + size.Width / 2; x <= viewport.Right; x += size.Width)
					for (float y = viewport.Top + size.Height / 2; y <= viewport.Bottom; y += size.Height)
						renderer.Add(CreateRabbitWith50Health(rabbitImage, new Point(x, y)));
			});
		}

		[VisualTest]
		public void SpawnDeadRabbitAtMousePosition(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer, InputCommands input) =>
			{
				var deadRabbitImage = content.Load<Image>("DeadRabbit");
				input.Add(MouseButton.Left,
					mouse =>
						renderer.Add(new DeadRabbit(deadRabbitImage,
							Rectangle.FromCenter(mouse.Position,
								deadRabbitImage.PixelSize / Score.QuadraticFullscreenSize))));
				if (testResolver != null)
				{
					testResolver.SetMouseButtonState(MouseButton.Left, State.Releasing, Point.Half);
					testResolver.AdvanceTimeAndExecuteRunners(1);
				}
			});
		}
	}
}