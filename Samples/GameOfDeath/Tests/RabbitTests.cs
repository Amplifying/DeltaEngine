using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	public class RabbitTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowSingleRabbit()
		{
				var rabbitImage = ContentLoader.Load<Image>("Rabbit");
				CreateRabbitWith50Health(rabbitImage, Point.Half);
		}

		[Test]
		public void DamageSingleRabbitToHalfHealth()
		{

			var rabbitImage = ContentLoader.Load<Image>("Rabbit");
				var rabbit = CreateRabbitWith50Health(rabbitImage, Point.Half);
				rabbit.DoDamage(25);
		}

		private static Rabbit CreateRabbitWith50Health(Image rabbitImage, Point position)
		{
			var rabbit = new Rabbit(rabbitImage, position);
			rabbit.SetHealth(50);
			return rabbit;
		}

		[Test]
		public void ShowManyRabbits()
		{
			var rabbitImage = ContentLoader.Load<Image>("Rabbit");
				var viewport = Resolve<ScreenSpace>().Viewport;
				var size = RabbitGrid.CellSize;
				for (float x = viewport.Left + size.Width / 2; x <= viewport.Right; x += size.Width)
					for (float y = viewport.Top + size.Height / 2; y <= viewport.Bottom; y += size.Height)
						CreateRabbitWith50Health(rabbitImage, new Point(x, y));
		}

		[Test]
		public void SpawnDeadRabbitAtMousePosition()
		{

			var deadRabbitImage = ContentLoader.Load<Image>("DeadRabbit");
				Resolve<InputCommands>().Add(MouseButton.Left,
					mouse =>
						new DeadRabbit(deadRabbitImage,
							Rectangle.FromCenter(mouse.Position,
								deadRabbitImage.PixelSize / Scoreboard.QuadraticFullscreenSize)));

				Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Releasing);
				resolver.AdvanceTimeAndExecuteRunners(1);
		}
	}
}