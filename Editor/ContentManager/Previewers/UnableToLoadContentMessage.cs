using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class UnableToLoadContentMessage 
	{
		public static void SendUnableToLoadContentMessage(string contentName)
		{
			Logger.Warning("Could not load " + contentName + ". Check if all content is available");
			var verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana, "Could not load " + contentName + ". Check if all content is available",
				Rectangle.One);
		}
	}
}