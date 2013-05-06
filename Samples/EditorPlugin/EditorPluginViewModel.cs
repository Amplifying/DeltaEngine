using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.EditorPlugin
{
	public class EditorPluginViewModel : ViewModelBase
	{
		public EditorPluginViewModel()
		{
			ClickCommand = new RelayCommand(Click);
		}

		public ICommand ClickCommand { get; private set; }

		private void Click()
		{
			button.Text = "Clicked";
			RaisePropertyChanged("Button");
		}

		private readonly MyButton button = new MyButton();

		public MyButton Button
		{
			get { return button; }
		}
	}
}