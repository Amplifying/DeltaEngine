using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	internal class UIEditorViewTests
	{
		[SetUp]
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
			uiEditorView = new UIEditorView(fileSystem);
		}

		private UIEditorView uiEditorView;

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		private RoutedEventArgs routedEvent;

		//this test has to be catagory slow because else there will be a chance for threading issues
		[Test, STAThread, Category("Slow")]
		public void ChangeSelectImageInGridList()
		{
			AddImageToGrid();
			AddImageToGrid();
			uiEditorView.SelectImageInGridList(null, CreateSelectionChangedEventArgs());
		}

		private static SelectionChangedEventArgs CreateSelectionChangedEventArgs()
		{
			RoutedEvent tapEvent = EventManager.RegisterRoutedEvent(
			 "Test", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIEditorViewModel));
			var newRoutedEvent = new RoutedEventArgs(tapEvent);
			var selectionChangedEvent = new SelectionChangedEventArgs(newRoutedEvent.RoutedEvent,
				new StringCollection(), new StringCollection());
			return selectionChangedEvent;
		}

		[Test, STAThread, Category("Slow")]
		public void ChangeIfButtonState()
		{
			AddImageToGrid();
			uiEditorView.ChangeIfButtonState(null, CreateSelectionChangedEventArgs2());
		}

		private static SelectionChangedEventArgs CreateSelectionChangedEventArgs2()
		{
			RoutedEvent tapEvent = EventManager.RegisterRoutedEvent(
			 "Test2", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIEditorViewModel));
			var newRoutedEvent = new RoutedEventArgs(tapEvent);
			var selectionChangedEvent = new SelectionChangedEventArgs(newRoutedEvent.RoutedEvent,
				new StringCollection(), new StringCollection());
			return selectionChangedEvent;
		}

		[Test, STAThread]
		public void AddingAnImage()
		{
			uiEditorView.AddImage(null, routedEvent);
		}

		private void AddImageToGrid() 
		{
			uiEditorView.uiEditorViewModel.ProjectName = "BreakOut";
			uiEditorView.uiEditorViewModel.SelectedImageInList = "DeltaEngineLogo.png";
			uiEditorView.uiEditorViewModel.AddImage("DeltaEngineLogo.png");
			uiEditorView.uiEditorViewModel.SelectedImageInGridIndex = 0;
		}

		[Test, STAThread]
		public void ChangeGridSnapping()
		{
			uiEditorView.ChangeGridSnapping(null, routedEvent);
			Assert.AreEqual(1, uiEditorView.uiEditorViewModel.PixelSnapgrid);
		}

		[Test, STAThread]
		public void ChangeGridWidth()
		{
			uiEditorView.ChangeGridWidth(null, routedEvent);
			Assert.AreEqual(50, uiEditorView.uiEditorViewModel.GridWidth);
		}

		[Test, STAThread]
		public void ChangeGridHeight()
		{
			uiEditorView.ChangeGridHeight(null, routedEvent);
			Assert.AreEqual(50, uiEditorView.uiEditorViewModel.GridHeight);
		}

		[Test, STAThread]
		public void LayerChanged()
		{
			uiEditorView.LayerChanged(null, routedEvent);
			Assert.AreEqual(50, uiEditorView.uiEditorViewModel.GridHeight);
		}

		[Test, STAThread, Category("slow")]
		public void SaveUI()
		{
			uiEditorView.Save(null, routedEvent);
		}

		[Test, STAThread, Category("slow")]
		public void LoadUI()
		{
			uiEditorView.Load(null, routedEvent);
		}

		[Test, STAThread, Category("slow")]
		public void RotateSliderChange()
		{
			AddImageToGrid();
			uiEditorView.RotateSliderChanged(null, CreateChangedEvent());
		}

		private RoutedPropertyChangedEventArgs<double> CreateChangedEvent()
		{
			var changeEvent = new RoutedPropertyChangedEventArgs<double>(0, 1);
			return changeEvent;
		}

		[Test, STAThread, Category("slow")]
		public void ScaleSliderChanged()
		{
			AddImageToGrid();
			uiEditorView.ScaleSliderChanged(null, CreateChangedEvent());
		}
	}
}