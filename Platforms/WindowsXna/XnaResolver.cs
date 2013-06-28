using System;
using DeltaEngine.Content.Online;
using DeltaEngine.Core;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Input;
using DeltaEngine.Input.Windows;
using DeltaEngine.Input.Xna;
using DeltaEngine.Logging;
using DeltaEngine.Logging.Basic;
using DeltaEngine.Multimedia.Xna;
using DeltaEngine.Networking.Sockets;
using DeltaEngine.Physics2D.Farseer;
using DeltaEngine.Rendering.ScreenSpaces;
using Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Platforms
{
	internal class XnaResolver : AutofacStarter
	{
		public XnaResolver()
		{
			game = new XnaGame(this);
			var window = new XnaWindow(game);
			RegisterInstance(window);
			var settings = new FileSettings();
			RegisterInstance(settings);
			var device = new XnaDevice(game, window, settings);
			RegisterInstance(device);
			game.StartXnaGameToInitializeGraphics();
			RegisterInstance(game);
			RegisterInstance(game.Content);
			new DeveloperOnlineContentLoader(new AutofacContentDataResolver(this));
			if (!(Time.Current is StopwatchTime))
				Time.Current = new StopwatchTime();
			if (!(Logger.Current is DefaultLogger))
				Logger.Current = new DefaultLogger(new TcpSocket(), settings);
			RegisterSingleton<QuadraticScreenSpace>();
			RegisterSingleton<InputCommands>();
			RegisterSingleton<FarseerPhysics>();
			RegisterSingleton<XnaSoundDevice>();
			RegisterSingleton<XnaScreenshotCapturer>();
			RegisterSingleton<VideoRenderingDependencies>();
			RegisterSingleton<VideoPlayer>();
			RegisterSingleton<XnaDrawing>();
			RegisterSingleton<XnaMouse>();
			RegisterSingleton<XnaKeyboard>();
			RegisterSingleton<XnaTouch>();
			RegisterSingleton<CursorPositionTranslater>();
			RegisterSingleton<XnaGamePad>();
			RegisterSingleton<XnaSystemInformation>();
		}

		private readonly XnaGame game;

		protected override void MakeSureContainerIsInitialized()
		{
			if (!IsAlreadyInitialized)
				RegisterInstanceAsRunnerOrPresenterIfPossible(Time.Current);

			base.MakeSureContainerIsInitialized();
		}

		/// <summary>
		/// Instead of starting the game normally and blocking we will delay the initialization in
		/// XnaGame until the game class has been constructed and the graphics device is available.
		/// </summary>
		public override void Run(Action optionalRunCode)
		{
			RaiseInitializedEvent();
			Resolve<XnaDevice>();
			Resolve<Window>();
			game.RunXnaGame(optionalRunCode);
		}
	}
}