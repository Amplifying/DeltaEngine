using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace LogoApp.Tests
{
	public class LogoGameTests
	{
		[Test]
		public void StartGameWithMocks()
		{
			TestAppOnce.Start((LogoGame game) => game.Run());
		}

		[Test]
		public void StartRunFewTimesAndCloseGame()
		{
			TestAppOnce.Start((LogoGame game) =>
			{
				for (int i = 0; i < 50; i++)
					game.Run();
			});
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void ShowGame()
		{
			App.Start<LogoGame>();
		}
	}
}