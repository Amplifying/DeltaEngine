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
		public override void Preview(string contentName)
		{
			verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana, "Play/Stop", Rectangle.One);
			music = ContentLoader.Load<Music>(contentName);
			music.Play(1);
			new Command(() => //ncrunch: no coverage start
			{
				if (music.IsPlaying())
					music.Stop();
				else
					music.Play(1);
			}).Add(new MouseButtonTrigger());
			//ncrunch: no coverage end
		}

		private Font verdana;
		public Music music;
	}
}