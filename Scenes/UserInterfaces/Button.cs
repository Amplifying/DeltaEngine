using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A simple UI button to which events can be attached when being pressed and released.
	/// Can also change color and/or image on mouseover and press.
	/// </summary>
	public class Button : InteractiveControl
	{
		public Button(Image image, Rectangle initialDrawArea)
			: base(image, initialDrawArea)
		{
			PressedImage = image;
			NormalImage = image;
			MouseoverImage = image;
		}

		public Image PressedImage { get; set; }
		public Image NormalImage { get; set; }
		public Image MouseoverImage { get; set; }

		private Button()
			: base(null, Rectangle.Zero) {}

		public override void Press()
		{
			base.Press();
			SetImageAndColor();
		}

		private void SetImageAndColor()
		{
			if (IsInside && IsPressed)
				SetPressedImageAndColor();
			else if (IsInside)
				SetMouseoverImageAndColor();
			else
				SetNormalImageAndColor();
		}

		private void SetPressedImageAndColor()
		{
			Image = PressedImage;
			Color = PressedColor;
		}

		public Color PressedColor
		{
			get { return pressedColor; }
			set
			{
				pressedColor = value;
				SetImageAndColor();
			}
		}

		private Color pressedColor = Color.White;

		private void SetMouseoverImageAndColor()
		{
			Image = MouseoverImage;
			Color = MouseoverColor;
		}

		public Color MouseoverColor
		{
			get { return mouseoverColor; }
			set
			{
				mouseoverColor = value;
				SetImageAndColor();
			}
		}

		private Color mouseoverColor = Color.White;

		private void SetNormalImageAndColor()
		{
			Image = NormalImage;
			Color = NormalColor;
		}

		public Color NormalColor
		{
			get { return normalColor; }
			set
			{
				normalColor = value;
				SetImageAndColor();
			}
		}

		private Color normalColor = Color.White;

		public override void Release()
		{
			base.Release();
			SetImageAndColor();
		}

		public override void Enter()
		{
			base.Enter();
			SetImageAndColor();
		}

		public override void Exit()
		{
			base.Exit();
			SetImageAndColor();
		}

		public override void LoadData(BinaryReader reader)
		{
			base.LoadData(reader);
			normalImageFilename = reader.ReadString();
			mouseoverImageFilename = reader.ReadString();
			pressedImageFilename = reader.ReadString();
			normalColor.LoadData(reader);
			mouseoverColor.LoadData(reader);
			pressedColor.LoadData(reader);
		}

		private string normalImageFilename;
		private string mouseoverImageFilename;
		private string pressedImageFilename;

		internal override void LoadContent(Content content)
		{
			base.LoadContent(content);
			if (NormalImage == null && !string.IsNullOrEmpty(normalImageFilename))
				NormalImage = content.Load<Image>(normalImageFilename);

			if (MouseoverImage == null && !string.IsNullOrEmpty(mouseoverImageFilename))
				MouseoverImage = content.Load<Image>(mouseoverImageFilename);

			if (PressedImage == null && !string.IsNullOrEmpty(pressedImageFilename))
				PressedImage = content.Load<Image>(pressedImageFilename);
		}

		public override void SaveData(BinaryWriter writer)
		{
			base.SaveData(writer);
			writer.Write(NormalImage.Filename);
			writer.Write(MouseoverImage.Filename);
			writer.Write(PressedImage.Filename);
			normalColor.SaveData(writer);
			mouseoverColor.SaveData(writer);
			pressedColor.SaveData(writer);
		}
	}
}