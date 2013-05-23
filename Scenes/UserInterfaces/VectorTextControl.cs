//TODO: VectorTextControl

//using DeltaEngine.Content;
//using DeltaEngine.Core.Xml;
//using DeltaEngine.Datatypes;
//using DeltaEngine.Rendering;

//namespace DeltaEngine.Scenes.UserInterfaces
//{
//	/// <summary>
//	/// This allows VectorText to be added to a Scene
//	/// </summary>
//	public class VectorTextControl : Control
//	{
//		public VectorTextControl(XmlContent vectorTextContent, Point topLeft, float height)
//		{
//			VectorText = new VectorText(vectorTextContent, topLeft, height);
//			VectorTextContentName = vectorTextContent.Name;
//		}

//		public VectorText VectorText { get; private set; }
//		internal string VectorTextContentName { get; private set; }

//		// ReSharper disable UnusedMember.Local
//		private VectorTextControl()
//		{
//			VectorText = new VectorText(new XmlData("Placeholder"), Point.Zero, 0.0f);
//		}

//		/*
//		public override void SaveData(BinaryWriter writer)
//		{
//			base.SaveData(writer);
//			writer.Write(VectorTextContentFilename);
//			writer.Write(VectorText.TopLeft.ToString());
//			writer.Write(VectorText.Text);
//			writer.Write(VectorText.Height);
//			VectorText.Color.SaveData(writer);
//			writer.Write(VectorText.RenderLayer);
//		public override void LoadData(BinaryReader reader)
//		{
//			base.LoadData(reader);
//			VectorTextContentFilename = reader.ReadString();
//			VectorText.TopLeft = new Point(reader.ReadString());
//			VectorText.Text = reader.ReadString();
//			VectorText.Height = reader.ReadSingle();
//			VectorText.Color.LoadData(reader);
//			VectorText.RenderLayer = reader.ReadInt32();
//		}
//		*/

//		internal override void LoadContent(ContentLoader content)
//		{
//			var vectorTextContent = content.Load<XmlContent>(VectorTextContentName);
//			VectorText = new VectorText(vectorTextContent, VectorText.TopLeft, VectorText.Height)
//			{
//				Color = VectorText.Color,
//				Visibility = VectorText.Visibility,
//				RenderLayer = VectorText.RenderLayer,
//				Text = VectorText.Text
//			};
//		}

//		internal override void Show(ObsRenderer renderer)
//		{
//			renderer.Add(VectorText);
//		}

//		internal override void Hide(ObsRenderer renderer)
//		{
//			renderer.Remove(VectorText);
//		}

//		public override void Dispose()
//		{
//			VectorText.Dispose();
//		}
//	}
//}