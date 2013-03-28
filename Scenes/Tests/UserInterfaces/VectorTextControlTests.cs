using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class VectorTextControlTests : TestStarter
	{
		[VisualTest]
		public void CreateSaveLoadAndShowRedHello(Type resolver)
		{
			Start(resolver, (Renderer renderer, Content content, InputCommands input) =>
			{
				var vectorText = CreateVectorTextControl(content);
				var scene = new Scene();
				scene.Add(vectorText);
				MemoryStream stream = scene.SaveToMemoryStream();
				CheckControlsMatch(vectorText,
					stream.CreateFromMemoryStream<Scene>().Find("VectorText") as VectorTextControl);
				scene.Show(renderer, content, input);
			});
		}

		private static VectorTextControl CreateVectorTextControl(Content content)
		{
			return new VectorTextControl(content.Load<XmlContent>("TestXml"), new Point(0.3f, 0.4f), 0.1f)
			{
				Color = Color.Red,
				IsVisible = true,
				Name = "VectorText",
				RenderLayer = 5,
				Text = "Hello"
			};
		}

		private static void CheckControlsMatch(VectorTextControl vectorText1,
			VectorTextControl vectorText2)
		{
			Assert.AreEqual(vectorText1.Color, vectorText2.Color);
			Assert.AreEqual(vectorText1.Height, vectorText2.Height);
			Assert.AreEqual(vectorText1.IsVisible, vectorText2.IsVisible);
			Assert.AreEqual(vectorText1.RenderLayer, vectorText2.RenderLayer);
			Assert.AreEqual(vectorText1.Text, vectorText2.Text);
			Assert.AreEqual(vectorText1.TopLeft, vectorText2.TopLeft);
			Assert.AreEqual(vectorText1.VectorTextContentFilename, vectorText2.VectorTextContentFilename);
		}
	}
}