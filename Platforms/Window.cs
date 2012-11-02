using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Window form the application is running in. In Windows this is done with Windows Forms or WPF.
	/// </summary>
	public interface Window : Runner, IDisposable
	{
		string Title { get; set; }
		bool IsVisible { get; }
		IntPtr Handle { get; }
		Size ViewportSize { get; }
		event Action<Size> ViewportSizeChanged;
		Size TotalSize { get; set; }
		Color BackgroundColor { get; set; }
		bool IsFullscreen { get; set; }
		bool IsClosing { get; }
	}
}