using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class SoundPreviewer : ContentPreview
	{
		public override void Preview(string contentName)
		{
			verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana, "Play", Rectangle.One);
			sound = ContentLoader.Load<Sound>(contentName);
			var trigger = new MouseButtonTrigger();
			trigger.AddTag("temporary");
			var soundCommand = new Command(() => sound.Play(1)).Add(trigger);
			soundCommand.AddTag("temporary");
		}

		private Font verdana;
		public Sound sound;
	}
}