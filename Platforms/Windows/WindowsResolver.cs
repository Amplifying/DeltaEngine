using DeltaEngine.Content.Disk;
using DeltaEngine.Core;
using DeltaEngine.Input;
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
			RegisterInstance(new AutofacContentDataResolver(this));
			RegisterSingleton<DiskContentLoader>();
			RegisterSingleton<FormsWindow>();
			RegisterSingleton<QuadraticScreenSpace>();
			RegisterSingleton<DefaultLogger>();
			RegisterSingleton<PointerDevices>();
			RegisterSingleton<InputCommands>();
			Register<TcpSocket>();
			RegisterSingleton<FarseerPhysics>();
		}

		protected override void MakeSureContainerIsInitialized()
		{
			if (!IsAlreadyInitialized)
				RegisterInstanceAsRunnerOrPresenterIfPossible(Time.Current);

			base.MakeSureContainerIsInitialized();
		}
	}
}