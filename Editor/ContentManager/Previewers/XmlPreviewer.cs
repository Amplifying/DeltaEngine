using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class XmlPreviewer : ContentPreview
	{
		//ncrunch: no coverage start
		public void PreviewContent(string contentName)
		{
			Process.Start(Directory.GetCurrentDirectory() + contentName);
		}
		//ncrunch: no coverage end
	}
}