//TODO: VectorTextControlTests

//using System;
//using System.IO;
//using DeltaEngine.Content;
//using DeltaEngine.Core.Xml;
//using DeltaEngine.Datatypes;
//using DeltaEngine.Input;
//using DeltaEngine.Platforms.All;
//using DeltaEngine.Rendering;
//using DeltaEngine.Scenes.UserInterfaces;
//using NUnit.Framework;

//namespace DeltaEngine.Scenes.Tests.UserInterfaces
//{
//	public class VectorTextControlTests : TestWithAllFrameworks
//	{
//		[VisualTest]
//		public void CreateSaveLoadAndShowRedHello(Type resolver)
//		{
//			Start(resolver, (ObsRenderer renderer, ContentLoader content, InputCommands input) =>
//			{
//				var vectorText = CreateVectorTextControl(content);
//				var scene = new Scene();
//				scene.Add(vectorText);
//				MemoryStream stream = scene.SaveToMemoryStream();
//				CheckControlsMatch(vectorText,
//					(stream.CreateFromMemoryStream() as Scene).Find("VectorText") as VectorTextControl);
//				scene.Show(renderer, content, input);
//			});
//		}

//		private static VectorTextControl CreateVectorTextControl(ContentLoader content)
//		{
//			return new VectorTextControl(content.Load<XmlContent>("VectorText"), new Point(0.3f, 0.4f),
//				0.1f)
//			{
//				Name = "VectorText",
//				VectorText = { Color = Color.Red, Visibility = true, RenderLayer = 5, Text = "Hello" }
//			};
//		}

//		private static void CheckControlsMatch(VectorTextControl vectorText1,
//			VectorTextControl vectorText2)
//		{
//			Assert.AreEqual(vectorText1.VectorText.Color, vectorText2.VectorText.Color);
//			Assert.AreEqual(vectorText1.VectorText.Height, vectorText2.VectorText.Height);
//			Assert.AreEqual(vectorText1.VectorText.Visibility, vectorText2.VectorText.Visibility);
//			Assert.AreEqual(vectorText1.VectorText.RenderLayer, vectorText2.VectorText.RenderLayer);
//			Assert.AreEqual(vectorText1.VectorText.Text, vectorText2.VectorText.Text);
//			Assert.AreEqual(vectorText1.VectorText.TopLeft, vectorText2.VectorText.TopLeft);
//			Assert.AreEqual(vectorText1.VectorTextContentName, vectorText2.VectorTextContentName);
//		}
//	}
//}