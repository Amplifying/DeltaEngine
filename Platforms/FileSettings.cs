using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;

namespace DeltaEngine.Platforms
{
	public class FileSettings : Settings
	{
		public FileSettings(bool allowToLoadFromContent = true)
		{
			if (File.Exists(SettingsFilename))
				data = new XmlFile(SettingsFilename).Root;
			else if (allowToLoadFromContent)
				TryToGenerateSettingsFromContentDefaultSettings();
			else
				SetFallbackSettings();
		}

		private void TryToGenerateSettingsFromContentDefaultSettings()
		{
			if (ContentLoader.Exists("DefaultSettings"))
				data = ContentLoader.Load<XmlContent>("DefaultSettings").Data;
			else
				SetFallbackSettings();
		}

		public override void Save()
		{
			new XmlFile(data).Save(SettingsFilename);
		}
	}
}