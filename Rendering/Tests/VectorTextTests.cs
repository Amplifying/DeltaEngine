using System;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	/// <summary>
	/// Unit tests for VectorText 
	/// </summary>
	public class VectorTextTests : TestStarter
	{
		[Test]
		public void KnownCharacterRendersWithoutException()
		{
			var resolver = new TestResolver();
			var renderer = resolver.Resolve<Renderer>();
			renderer.Add(new VectorText(CreateVectorTextData(), Point.Zero, 1.0f) { Text = "A" });
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
		}

		private static XmlData CreateVectorTextData()
		{
			var vectorTextData = new XmlData("VectorText");
			var character = new XmlData("Char1");
			character.AddAttribute("Character", "A");
			character.AddAttribute("Lines", "(0,0)-(1,1)");
			vectorTextData.AddChild(character);
			return vectorTextData;
		}

		[Test]
		public void UnknownCharacterThrowsException()
		{
			var resolver = new TestResolver();
			var renderer = resolver.Resolve<Renderer>();
			renderer.Add(new VectorText(CreateVectorTextData(), Point.Zero, 1.0f) { Text = "B" });
			Assert.Throws<VectorText.VectorCharacterNotFoundException>(
				() => renderer.Run(resolver.Resolve<Time>()));
		}

		[VisualTest]
		public void DrawSampleText(Type resolver)
		{
			Start(resolver, (Renderer renderer, Content content) =>
			{
				var vectorTextContent = content.Load<XmlContent>("VectorText");
				renderer.Add(new VectorText(vectorTextContent, new Point(0.1f, 0.45f), 0.05f)
				{
					Text = "Blue0123456789",
					Color = Color.Blue
				});

				renderer.Add(new VectorText(vectorTextContent, new Point(0.1f, 0.5f), 0.05f)
				{
					Text = "The Quick Brown Fox..."
				});

				renderer.Add(new VectorText(vectorTextContent, new Point(0.1f, 0.55f), 0.05f)
				{
					Text = "Jumps Over The Lazy Dog"
				});
			});
		}
	}
}