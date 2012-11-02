using System;
using DeltaEngine.Core;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Xna game to just call all runners and presenters, but no xna components!
	/// </summary>
	internal sealed class XnaGame : Game
	{
		internal XnaGame(Resolver resolver)
		{
			this.resolver = resolver;
			IsFixedTimeStep = false;
		}

		private readonly Resolver resolver;

		internal void Run(Action runCode)
		{
			runCodeForTests = runCode;
			Run();
		}

		private Action runCodeForTests;

		protected override void Update(GameTime gameTime)
		{
			if (runCodeForTests != null)
				runCodeForTests();

			resolver.RunAllRunners();
		}

		protected override void Draw(GameTime gameTime)
		{
			resolver.RunAllPresenters();
		}
	}
}