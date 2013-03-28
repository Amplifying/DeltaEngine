using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// This allows VectorText to be added to a Scene
	/// </summary>
	public class VectorTextControl : Control
	{
		public VectorTextControl(XmlContent vectorTextContent, Point topLeft, float height)
		{
			vectorText = new VectorText(vectorTextContent, topLeft, height);
			VectorTextContentFilename = vectorTextContent.Filename;
		}

		private VectorText vectorText;
		internal string VectorTextContentFilename { get; private set; }

		private VectorTextControl()
		{
			vectorText = new VectorText(new XmlData("Placeholder"), Point.Zero, 0.0f);
		}

		public override void SaveData(BinaryWriter writer)
		{
			base.SaveData(writer);
			writer.Write(VectorTextContentFilename);
			writer.Write(TopLeft.ToString());
			writer.Write(Text);
			writer.Write(Height);
			Color.SaveData(writer);
		}

		public Point TopLeft
		{
			get { return vectorText.TopLeft; }
			set { vectorText.TopLeft = value; }
		}

		public string Text
		{
			get { return vectorText.Text; }
			set { vectorText.Text = value; }
		}

		public float Height
		{
			get { return vectorText.Height; }
			set { vectorText.Height = value; }
		}

		public Color Color
		{
			get { return vectorText.Color; }
			set { vectorText.Color = value; }
		}

		public override int RenderLayer
		{
			get { return vectorText.RenderLayer; }
			set { vectorText.RenderLayer = value; }
		}

		public override bool IsVisible
		{
			get { return vectorText.IsVisible; }
			set { vectorText.IsVisible = value; }
		}

		public override void LoadData(BinaryReader reader)
		{
			base.LoadData(reader);
			VectorTextContentFilename = reader.ReadString();
			TopLeft = new Point(reader.ReadString());
			Text = reader.ReadString();
			Height = reader.ReadSingle();
			vectorText.Color.LoadData(reader);
		}

		internal override void LoadContent(Content content)
		{
			var vectorTextContent = content.Load<XmlContent>(VectorTextContentFilename);
			vectorText = new VectorText(vectorTextContent, TopLeft, Height)
			{
				Color = Color,
				IsVisible = IsVisible,
				RenderLayer = RenderLayer,
				Text = Text
			};
		}

		internal override void Show(Renderer renderer)
		{
			renderer.Add(vectorText);
		}

		internal override void Hide(Renderer renderer)
		{
			renderer.Remove(vectorText);
		}

		public override void Dispose()
		{
			vectorText.Dispose();
		}
	}
}