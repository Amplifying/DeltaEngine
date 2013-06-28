using DeltaEngine.Content;
using DeltaEngine.Content.Online;
using DeltaEngine.Core;
using DeltaEngine.Input;
using DeltaEngine.Logging;
using DeltaEngine.Logging.Basic;
using DeltaEngine.Networking.Sockets;
using DeltaEngine.Physics2D.Farseer;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Platforms.Windows
{
	/// <summary>
	/// Base resolver class for all windows framework implementations (OpenGL, DirectX or XNA).
	/// </summary>
	public abstract class WindowsResolver : AutofacStarter
	{
		protected WindowsResolver()
		{
			new DeveloperOnlineContentLoader(new AutofacContentDataResolver(this));
			if (!(Time.Current is StopwatchTime))
				Time.Current = new StopwatchTime();
			var settings = new FileSettings();
			if (!(Logger.Current is DefaultLogger))
				Logger.Current = new DefaultLogger(new TcpSocket(), settings);
			RegisterSingleton<FileSettings>();
			RegisterSingleton<FormsWindow>();
			RegisterSingleton<QuadraticScreenSpace>();
			RegisterSingleton<InputCommands>();
			RegisterSingleton<FarseerPhysics>();
			RegisterSingleton<WindowsSystemInformation>();
		}

		protected override void MakeSureContainerIsInitialized()
		{
			if (!IsAlreadyInitialized)
				RegisterInstanceAsRunnerOrPresenterIfPossible(Time.Current);

			base.MakeSureContainerIsInitialized();
		}
	}
}