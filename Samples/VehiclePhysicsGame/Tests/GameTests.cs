using DeltaEngine.Input;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using DeltaEngine.Content;

namespace VehiclePhysicsGame.Tests
{
	class GameTests : TestWithAllFrameworks
	{
		[Test]
		public void StartWholeGame()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, Physics physics, ScreenSpace screen, InputCommands inputCommands) =>
			{ new VehiclePhysicsGame(contentLoader, screen, physics, inputCommands); });
		}
	}
}
