using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class XmlPreviewer : ContentPreview
	{
		//ncrunch: no coverage start
		public void PreviewContent(string contentName)
		{
			Process.Start(Directory.GetCurrentDirectory() + "\\Content\\LogoApp\\" + contentName +
				".xml");
		}
		//ncrunch: no coverage end
	}
}