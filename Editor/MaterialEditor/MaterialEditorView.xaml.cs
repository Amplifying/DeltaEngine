using System;
using System.Windows;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.ImageAnimationEditor;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.MaterialEditor
{
	/// <summary>
	/// Interaction logic for MaterialEditorView.xaml
	/// </summary>
	public partial class MaterialEditorView : EditorPluginView
	{
		//ncrunch: no coverage start
		public MaterialEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			current = new MaterialEditorViewModel(service);
			this.service = service;
			service.ProjectChanged += () => Dispatcher.Invoke(new Action(current.ResetOnProjectChange));
			service.ContentUpdated +=
				(type, s) => Dispatcher.Invoke(new Action(current.RefreshOnContentChange));
			service.ContentDeleted += s => Dispatcher.Invoke(new Action(current.RefreshOnContentChange));
			DataContext = current;
			Messenger.Default.Send("MaterialEditor", "SetSelectedEditorPlugin");
		}

		public void Activate()
		{
			current.Activate();
			Messenger.Default.Send("MaterialEditor", "SetSelectedEditorPlugin");
		}

		private MaterialEditorViewModel current;
		private Service service;

		public string ShortName
		{
			get { return "Material Editor"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Material.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void SaveMaterial(object sender, RoutedEventArgs e)
		{
			current.Save();
		}

		private void OpenAnimationEditor(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(AnimationEditorView));
		}

		private void ButtonBaseOnClick(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(ContentManagerView));
		}
	}
}