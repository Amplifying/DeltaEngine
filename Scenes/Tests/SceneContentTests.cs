using System;
using DeltaEngine.Content;
using DeltaEngine.Platforms.All;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	/// <summary>
	/// Confirms SceneContent can load from file
	/// </summary>
	public class SceneContentTests : TestWithAllFrameworks
	{
		[Test, Ignore]
		public void LoadAndDisposeSceneContentFromFile()
		{
			using (new SceneContentFile("TestScene")) {}
		}

		[IntegrationTest, Ignore]
		public void LoadSceneContentFromFileUsingContentSystem(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				Scene testScene = content.Load<SceneContent>("TestScene").Scene;
				var label = (Label)testScene.Find("Label");
				Assert.AreEqual(0.45f, label.Sprite.DrawArea.Left);
			});
		}
	}
}