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
		public WpfHostedFormsWindow(EmulatorControl emulatorControl, System.Windows.Window window)
			: base(GetEmulatorControlPanel(emulatorControl))
		{
			dispatcher = emulatorControl.ViewportHost.Dispatcher;
			this.window = window;
			emulatorControl.ViewportHost.SizeChanged += OnHostControlSizeChanged;
			ScreenSpace.Current = new QuadraticScreenSpace(this);
		}

		private static Panel GetEmulatorControlPanel(EmulatorControl emulatorControl)
		{
			emulatorControl.ApplyEmulator();
			return emulatorControl.Screen;
		}

		private readonly Dispatcher dispatcher;
		private readonly System.Windows.Window window;

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