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
		public XnaGame(Resolver resolver, Action initCode)
		{
			this.resolver = resolver;
			this.initCode = initCode;
			IsFixedTimeStep = false;
		}

		private readonly Resolver resolver;
		private readonly Action initCode;

		protected override void Initialize()
		{
			base.Initialize();
			initCode();
		}
		
		internal void Run(Action runCode)
		{
			runCodeForTests = runCode;
			Run();
		}

		private Action runCodeForTests;

		protected override void Update(GameTime gameTime) {}

		protected override void Draw(GameTime gameTime)
		{
			resolver.RunAllRunners();
			if (runCodeForTests != null)
				runCodeForTests();
			resolver.RunAllPresenters();
		}
	}
}