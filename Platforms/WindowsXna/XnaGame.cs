using System;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Xna game to just call all runners and presenters, but no xna components!
	/// </summary>
	internal sealed class XnaGame : Game
	{
		public XnaGame(XnaResolver resolver)
		{
			this.resolver = resolver;
			IsFixedTimeStep = false;
		}

		private readonly XnaResolver resolver;

		/// <summary>
		/// For the initialization code following this call we need a initialized graphics device.
		/// We cannot call Run because it would block the App constructor, but we need the Game.Run
		/// code to continue execution. StartGameLoop does the same as Run, but does not block.
		/// </summary>
		public void StartXnaGameToInitializeGraphics()
		{
			var startGameLoopMethod = GetType().GetMethod("StartGameLoop",
				BindingFlags.NonPublic | BindingFlags.Instance);
			startGameLoopMethod.Invoke(this, null);
		}

		/// <summary>
		/// This is the continuation of the StartXnaGameToInitializeGraphics method above. Here we
		/// continue what Game.Run would have normally done. This blocks until the window is closed.
		/// </summary>
		public void RunXnaGame(Action optionalRunCode)
		{
			runCode = optionalRunCode;
			var gameHostField = GetType().BaseType.GetField("host",
				BindingFlags.NonPublic | BindingFlags.Instance);
			var runMethod = gameHostField.FieldType.GetMethod("Run",
				BindingFlags.NonPublic | BindingFlags.Instance);
			runMethod.Invoke(gameHostField.GetValue(this), null);
		}

		private Action runCode;

		protected override void Update(GameTime gameTime) {}

		protected override void Draw(GameTime gameTime)
		{
			resolver.TryRunAllRunnersAndPresenters(runCode);
		}

		public new void Dispose()
		{
			base.Dispose();
		}
	}
}