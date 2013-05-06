using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Window form the application is running in. In Windows this is done with Windows Forms or WPF.
	/// </summary>
	public interface Window : Presenter, IDisposable
	{
		string Title { get; set; }
		bool Visibility { get; }
		IntPtr Handle { get; }
		Size ViewportPixelSize { get; }
		event Action<Size> ViewportSizeChanged;
		event Action<Orientation> OrientationChanged;
		event Action<Size, bool> FullscreenChanged;
		Size TotalPixelSize { get; set; }
		Point PixelPosition { get; }
		Color BackgroundColor { get; set; }
		bool IsFullscreen { get; }
		void SetFullscreen(Size displaySize);
		void SetWindowed();
		bool IsClosing { get; }
		bool ShowCursor { get; set; }
		MessageBoxButton ShowMessageBox(string title, string message, MessageBoxButton buttons);
	}
}