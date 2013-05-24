using System.Windows.Media;
using System.Windows.Media.Imaging;
using DeltaEngine.Datatypes;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIImage : ViewModelBase
	{
		public UIImage(float x, float y, float width, float height, BitmapImage link, string fileName,
			string project)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			BitmapSource = link;
			IsButton = false;
			Layer = 10;
			FileName = fileName;
			Project = project;
			CreateTransform(width, height);
		}

		private void CreateTransform(float width, float height)
		{
			CreateNewRotationAndScaleTransform(width, height);
			Transform = new TransformGroup();
			Transform.Children.Add(Rotate);
			Transform.Children.Add(Scale);
		}

		private void CreateNewRotationAndScaleTransform(float width, float height)
		{
			Rotate = new RotateTransform();
			Rotate.CenterX = width / 2;
			Rotate.CenterY = height / 2;
			Rotate.Angle = 0;
			Scale = new ScaleTransform();
			Scale.CenterX = width / 2;
			Scale.CenterY = height / 2;
			Scale.ScaleX = 1;
			Scale.ScaleY = 1;
		}

		public float Width { get; set; }
		public float Height { get; set; }
		public bool IsButton { get; set; }
		public BitmapImage BitmapSource { get; set; }
		public string FileName { get; set; }
		public int Layer;

		public float X
		{
			get { return x; }
			set
			{
				x = value;
				RaisePropertyChanged("X");
			}
		}

		private float x;

		public float Y
		{
			get { return y; }
			set
			{
				y = value;
				RaisePropertyChanged("Y");
			}
		}

		private float y;
		public RotateTransform Rotate { get; set; }
		public ScaleTransform Scale { get; set; }
		public string Project { get; set; }
		public TransformGroup Transform
		{
			get { return transform; }
			set
			{
				transform = value;
				RaisePropertyChanged("Transform");
			}
		}
		private TransformGroup transform;

		public UIImage(Point position, float width, float height, BitmapImage link, string fileName,
			string project, int angle, float scale, int layer, bool isButton)
		{
			X = position.X;
			Y = position.Y;
			Width = width;
			Height = height;
			BitmapSource = link;
			IsButton = isButton;
			Layer = layer;
			FileName = fileName;
			Project = project;
			CreateTransform(width, height);
			Rotate.Angle = angle;
			Scale.ScaleX = scale;
			Scale.ScaleY = scale;
		}
	}
}