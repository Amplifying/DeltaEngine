using System;
using DeltaEngine.Content.Disk;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Input.Windows;
using DeltaEngine.Input.Xna;
using DeltaEngine.Multimedia.Xna;
using DeltaEngine.Platforms.Windows;
using Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Platforms
{
	internal class XnaResolver : WindowsResolver
	{
		public XnaResolver()
		{
			game = new XnaGame(this, RaiseInitializedEvent);
			RegisterInstance(game);
			RegisterInstance(game.Content);
			RegisterInstance(new AutofacContentDataResolver(this));
			RegisterSingleton<DiskContentLoader>();
			RegisterSingleton<XnaWindow>();
			RegisterSingleton<XnaSoundDevice>();
			RegisterSingleton<XnaScreenshotCapturer>();
			RegisterSingleton<XnaDevice>();
			RegisterSingleton<VideoRenderingDependencies>();
			RegisterSingleton<VideoPlayer>();
			RegisterSingleton<XnaDrawing>();
			RegisterSingleton<XnaMouse>();
			RegisterSingleton<XnaKeyboard>();
			RegisterSingleton<XnaTouch>();
			RegisterSingleton<CursorPositionTranslater>();
			RegisterSingleton<XnaGamePad>();
		}

		private readonly XnaGame game;

		/// <summary>
		/// Instead of starting the game normally and blocking we will delay the initialization in
		/// XnaGame until the game class has been constructed and the graphics device is available.
		/// </summary>
		public override void Run(Action runCode = null)
		{
			Resolve<XnaDevice>();
			game.Run(runCode);
		}
	}
}