using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using DeltaEngine.Platforms.Windows;
using DeltaEngine.ScreenSpaces;
using Application = System.Windows.Application;
using DeltaSize = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Used for rendering within the Editor
	/// </summary>
	public class WpfHostedFormsWindow : FormsWindow
	{
		public WpfHostedFormsWindow(ViewportControl viewportControl, Window window)
			: base(GetViewportControlPanel(viewportControl))
		{
			dispatcher = viewportControl.ViewportHost.Dispatcher;
			this.window = window;
			viewportControl.ViewportHost.SizeChanged += OnHostControlSizeChanged;
			new QuadraticScreenSpace(this);
		}

		private static Panel GetViewportControlPanel(ViewportControl viewportControl)
		{
			viewportControl.ApplyEmulator();
			return viewportControl.Screen;
		}

		private readonly Dispatcher dispatcher;
		private readonly Window window;

		private void OnHostControlSizeChanged(object s, SizeChangedEventArgs e)
		{
			OnSizeChanged(null, EventArgs.Empty);
		}

		public override void Dispose()
		{
			base.Dispose();
			dispatcher.InvokeShutdown();
			if (Application.Current != null)
				Application.Current.Shutdown();
		}

		protected override void ResizeCentered(DeltaSize newSizeInPixels)
		{
			throw new ResizeNotAllowedForInEditorViewport();
		}

		public class ResizeNotAllowedForInEditorViewport : Exception {}

		public override void SetFullscreen(DeltaSize setFullscreenViewportSize)
		{
			throw new ResizeNotAllowedForInEditorViewport();
		}

		public override void Present()
		{
			base.Present();
			ForceRescaleOnFirstFrameToFixBlackBlocksNotRefreshingWpfWindow();
		}

		private void ForceRescaleOnFirstFrameToFixBlackBlocksNotRefreshingWpfWindow()
		{
			if (forceRescaleOnce)
				return;
			forceRescaleOnce = true;
			window.Height = window.Height + 1;
		}

		private bool forceRescaleOnce;
	}
}