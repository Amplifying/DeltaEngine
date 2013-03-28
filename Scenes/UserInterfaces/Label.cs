using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// The simplest UI control which simply draws a Sprite on screen.
	/// </summary>
	public class Label : Control
	{
		public Label(Image image, Rectangle initialDrawArea)
		{
			sprite = new Sprite(image, initialDrawArea);
			ImageFilename = sprite.Image == null ? null : sprite.Image.Filename;
		}

		private readonly Sprite sprite;
		internal string ImageFilename { get; private set; }

		private Label()
		{
			sprite = new Sprite(null, Rectangle.Zero);
		}

		public override void SaveData(BinaryWriter writer)
		{
			base.SaveData(writer);
			writer.Write(Image.Filename);
			writer.Write(DrawArea.ToString());
			Color.SaveData(writer);
			writer.Write(Rotation);
			writer.Write((int)Flip);
			writer.Write(RotationCenter.ToString());
		}

		internal Image Image
		{
			get { return sprite.Image; }
			set
			{
				sprite.SetImage(value);
				ImageFilename = sprite.Image == null ? null : sprite.Image.Filename;
			}
		}

		public Rectangle DrawArea
		{
			get { return sprite.DrawArea; }
			set { sprite.DrawArea = value; }
		}

		public Color Color
		{
			get { return sprite.Color; }
			set { sprite.Color = value; }
		}

		public float Rotation
		{
			get { return sprite.Rotation; }
			set { sprite.Rotation = value; }
		}

		public FlipMode Flip
		{
			get { return sprite.Flip; }
			set { sprite.Flip = value; }
		}

		public Point RotationCenter
		{
			get { return sprite.RotationCenter; }
			set { sprite.RotationCenter = value; }
		}

		public override int RenderLayer
		{
			get { return sprite.RenderLayer; }
			set { sprite.RenderLayer = value; }
		}

		public override bool IsVisible
		{
			get { return sprite.IsVisible; }
			set { sprite.IsVisible = value; }
		}

		public override void LoadData(BinaryReader reader)
		{
			base.LoadData(reader);
			ImageFilename = reader.ReadString();
			DrawArea = new Rectangle(reader.ReadString());
			sprite.Color = new Color();
			sprite.Color.LoadData(reader);
			Rotation = reader.ReadSingle();
			Flip = (FlipMode)reader.ReadInt32();
			RotationCenter = new Point(reader.ReadString());
		}

		internal override void LoadContent(Content content)
		{
			Image = content.Load<Image>(ImageFilename);
		}

		internal override void Show(Renderer renderer)
		{
			renderer.Add(sprite);
		}

		internal override void Hide(Renderer renderer)
		{
			renderer.Remove(sprite);
		}

		public override void Dispose()
		{
			sprite.Dispose();
		}
	}
}