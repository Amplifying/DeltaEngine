using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Windows;
using System.Windows.Input;
using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	public class ContentManagerViewTests
	{
		[SetUp, STAThread]
		public void Setup()
		{
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						@"Content\BreakOut\DeltaEngineLogo.png",
						new MockFileData(DataToString(@"Content\DeltaEngineLogo.png"))
					},
				});
			manager =
				new ContentManagerView(new ContentManagerViewModel(new ContentServiceMock(fileSystem)));
			CreateEvents();
		}

		private ContentManagerView manager;

		private void CreateEvents()
		{
			eWheel = new MouseWheelEventArgs(Mouse.PrimaryDevice, 0, 1)
			{
				RoutedEvent = Mouse.MouseUpEvent
			};
			eMouse = new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseUpEvent };
			eButton = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
			{
				RoutedEvent = Mouse.MouseUpEvent
			};
		}

		private MouseWheelEventArgs eWheel;
		private MouseEventArgs eMouse;
		private MouseButtonEventArgs eButton;

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		[Test, STAThread]
		public void MouseLeftButtonUp()
		{
			manager.OnMouseLeftButtonUp(null, eButton);
		}

		[Test, STAThread]
		public void MouseLeftButtonDown()
		{
			manager.OnMouseLeftButtonDown(null, eButton);
		}

		[Test, STAThread]
		public void PreviewMouseWheelGoingUp()
		{
			manager.OnPreviewMouseWheel(null, eWheel);
		}

		[Test, STAThread]
		public void PreviewMouseWheelGoingDown()
		{
			manager.OnPreviewMouseWheel(null, eWheel);
		}

		[Test, STAThread]
		public void ScrollViewerScrollChanged()
		{
			manager.OnMouseMove(null, eMouse);
		}

		[Test, STAThread, Category("Slow")]
		public void CreateTestWindow()
		{
			var window = new Window { Content = manager, Width = 800, Height = 600 };
			window.Show();
			manager.OnMouseLeftButtonDown(null, eButton);
		}

		[Test, STAThread, Category("Slow")]
		public void ChangeMousePositionValues()
		{
			var window = new Window { Content = manager, Width = 800, Height = 600 };
			manager.lastCenterPositionOnTarget = new Point(1, 1);
			manager.targetBefore = new Point(1, 1);
			window.Show();
			manager.OnMouseLeftButtonDown(null, eButton);
			window.Show();
		}
	}
}