using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	/// <summary>
	/// Confirms SceneContent can load from file
	/// </summary>
	public class SceneContentTests : TestStarter
	{
		[Test, Ignore]
		public void LoadAndDisposeSceneContentFromFile()
		{
			using (new SceneContentFile("TestScene")) {}
		}

		[IntegrationTest, Ignore]
		public void LoadSceneContentFromFileUsingContentSystem(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				Scene testScene = content.Load<SceneContent>("TestScene").Scene;
				var label = (Label)testScene.Find("Label");
				Assert.AreEqual(0.45f, label.DrawArea.Left);
			});
		}
	}
}