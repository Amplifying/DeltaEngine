using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeltaEngine.Content;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class ContentViewerTest : TestWithMocksOrVisually
	{
		[Test]
		public void CreatPreviewersAndDisplayImage()
		{
			var contentViewer = new ContentViewer();
			contentViewer.View("DeltaEngineLogo", ContentType.Image);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Sprite>().Count);
		}

		[Test]
		public void TryingToDisplaySHaderWillFail()
		{
			var contentViewer = new ContentViewer();
			contentViewer.View("Position2DUv", ContentType.Shader);
		}
	}
}
