using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Shapes;
using Color = DeltaEngine.Datatypes.Color;
using EngineApp = DeltaEngine.Platforms.App;
using Point = DeltaEngine.Datatypes.Point;
using Window = DeltaEngine.Core.Window;

namespace DeltaEngine.Editor.Emulator.Tests
{
	/// <summary>
	/// Hosts the Viewport as UserControl
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			Show();
			var window = new WpfHostedFormsWindow(Test.EngineViewport, this);
			Closing += (sender, args) => window.Dispose();
			new BlockingViewportApp(window);
		}

		private class BlockingViewportApp : EngineApp
		{
			public BlockingViewportApp(Window windowToRegister)
				: base(windowToRegister)
			{
				Run();
			}
		}

		private void OnClickButton(object sender, RoutedEventArgs e)
		{
			TestButton.Background = new SolidColorBrush(Colors.Yellow);
			new Line2D(new Point(0, 1), new Point(Time.Total / 5f, 0), Color.Yellow);
		}

		public void Connect(int connectionId, object target) {}
	}
}