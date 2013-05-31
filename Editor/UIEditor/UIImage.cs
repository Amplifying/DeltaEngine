using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIImage : ViewModelBase
	{
		public UIImage(Rectangle rectangle, BitmapImage image, string fileName)
		{
			X = rectangle.Left;
			Y = rectangle.Top;
			Width = rectangle.Width;
			Height = rectangle.Height;
			BitmapSource = image;
			IsButton = false;
			Layer = 10;
			FileName = fileName;
			CreateTransform(rectangle.Width, rectangle.Height);
		}

		public UIImage(XmlData xmlData, BitmapImage image)
		{
			CreateTransform((float)image.Width, (float)image.Height);
			SetVisualImageData(xmlData, image);
			BitmapSource = image;
			Project = xmlData.GetAttributeValue("SelectedProject");
			IsButton = Convert.ToBoolean(xmlData.GetAttributeValue("IsButton"));
			Layer = Convert.ToInt32(xmlData.GetAttributeValue("Layer"));
			FileName = xmlData.GetAttributeValue("FileName");
		}

		private void SetVisualImageData(XmlData xmlData, BitmapImage image)
		{
			X = Convert.ToInt32(xmlData.GetAttributeValue("PositionX"));
			Y = Convert.ToInt32(xmlData.GetAttributeValue("PositionY"));
			Width = (float)image.Width;
			Height = (float)image.Height;
			Rotate.Angle = Convert.ToInt32(xmlData.GetAttributeValue("RotationAngle"));
			Scale.ScaleX = Convert.ToSingle(xmlData.GetAttributeValue("Scale"));
			Scale.ScaleY = Convert.ToSingle(xmlData.GetAttributeValue("Scale"));
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
		public string Project { get; set; }
	}
}