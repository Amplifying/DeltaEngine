using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class MusicPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana, "Play", Rectangle.One);
			music = ContentLoader.Load<Music>(contentName);
			music.Play(1);
			new Command(() => { music.Play(1); }).Add(new MouseButtonTrigger());
		}

		private Font verdana;
		public Music music;
	}
}