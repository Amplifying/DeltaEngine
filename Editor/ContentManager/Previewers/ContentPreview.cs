using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public abstract class ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			try
			{
				Preview(contentName);
			}
			catch (ContentLoader.ContentNotFound ex)
			{
				ShowFailToLoadContentMessage(contentName, ex);
			}
			catch (ContentLoader.ContentNameShouldNotHaveExtension ex)
			{
				ShowFailToLoadContentMessage(contentName, ex);
			}
			catch (Exception)
			{
				ShowFailToLoadMessage(contentName);
			}
		}

		private static void ShowFailToLoadContentMessage(string contentName,
			ContentLoader.ContentNotFound ex)
		{
			Logger.Warning("Content " + ex.Message + " in " + contentName + " not found");
			var verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana, "Content " + ex.Message + " in " + contentName + " not found",
				Rectangle.One);
		}

		private static void ShowFailToLoadContentMessage(string contentName,
			ContentLoader.ContentNameShouldNotHaveExtension ex)
		{
			Logger.Warning("Content " + contentName +
				" has an '.' in the contentName. This is not allowed for it will be viewed as an extension");
			var verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana,
				"Content " + contentName +
					" has an '.' in the contentName. This is not allowed for it will be viewed as an extension",
				Rectangle.One);
		}

		private static void ShowFailToLoadMessage(string contentName)
		{
			Logger.Warning("Could not load " + contentName + ". Check if all content is available");
			var verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana,
				"Could not load " + contentName + ". Check if all content is available", Rectangle.One);
		}

		public abstract void Preview(string contentName);
	}
}