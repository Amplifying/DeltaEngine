using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class XmlPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			Process.Start(Directory.GetCurrentDirectory() + "\\Content\\LogoApp\\" + contentName +
				".xml");
		}
	}
}