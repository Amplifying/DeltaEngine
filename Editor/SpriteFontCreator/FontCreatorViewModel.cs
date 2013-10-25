using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight;
using Microsoft.Win32;

namespace DeltaEngine.Editor.SpriteFontCreator
{
	public class FontCreatorViewModel : ViewModelBase
	{
		public FontCreatorViewModel()
		{
			settings = new FontGeneratorSettings();
			BestFontSize = 12;
			UseDefaultFont = true;
			settings.OutlineThicknessPercent = 0;
			UpdateAvailableDefaultFonts();
		}

		public FontCreatorViewModel(Service service)
			: this()
		{
			this.service = service;
			targetProjectPath = Path.Combine("Content", service.ProjectName);
		}

		private readonly Service service;
		public FontGeneratorSettings settings;
		public string targetProjectPath;

		public void OpenImportdialogue()
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "True Type Font |*.ttf";//TODO: "ttf", "otf", "pfa", "pfb", "pt3", "sfd", "otb", "t42", "cef", "cff", "gsf", "ttc", "svg", "ik", "mf", "dfont", "bdf",
			var result = dialog.ShowDialog();
			if (result == true)
				FamilyFilename = dialog.FileName;
		}

		public void GenerateFontFromSettings()
		{
			try
			{
				SendForGeneration();
			}
			catch (Exception ex)
			{
				Logger.Info(ex.ToString());
			}
		}

		private void SendForGeneration()
		{
			if (string.IsNullOrEmpty(ContentName))
				throw new CannotSaveFontWithoutSpecifiedContentName();
			if (string.IsNullOrEmpty(FamilyFilename))
				throw new GettingFontWithEmptyNameNotPossible();
			var metaDataToSend = SetMetaDataForFont();
			byte[] fontFileData;
			using (var fontFileReader = new BinaryReader(new FileStream(FamilyFilename, FileMode.Open)))
				fontFileData = fontFileReader.ReadBytes((int)fontFileReader.BaseStream.Length);
			var dataAndName = new Dictionary<string, byte[]> { { ContentName + ".ttf", fontFileData } };
			service.UploadContent(metaDataToSend, dataAndName);
		}

		private ContentMetaData SetMetaDataForFont()
		{
			var metaDataToSend = new ContentMetaData { Name = ContentName, Type = ContentType.Font };
			metaDataToSend.Values.Add("FontSize", BestFontSize.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("FontStyle", settings.Style.ToString());
			metaDataToSend.Values.Add("FontColor", settings.FontColor.ToString());
			metaDataToSend.Values.Add("ShadowColor", settings.ShadowColor.ToString());
			metaDataToSend.Values.Add("OutlineColor", settings.OutlineColor.ToString());
			metaDataToSend.Values.Add("OutlineThickness",
				settings.OutlineThicknessPercent.ToString(CultureInfo.InvariantCulture));
			//TODO: isn't more data missing? view has more data and there is also advanced data!
			return metaDataToSend;
		}

		public class CannotSaveFontWithoutSpecifiedContentName : Exception {}

		public class GettingFontWithEmptyNameNotPossible : Exception {}

		public void UpdateAvailableDefaultFonts()
		{
			var systemFonts = new InstalledFontCollection();
			for (int i = 0; i < systemFonts.Families.Length; i++)
				AvailableDefaultFontNames.Add(systemFonts.Families[i].Name);
		}

		public readonly List<string> AvailableDefaultFontNames = new List<string>();

		public string ContentName { get; set; }
		public string FamilyFilename
		{
			get { return familyFilename; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					familyFilename = value;
				RaisePropertyChanged("FamilyFilename");
			}
		}
		private string familyFilename;
		public bool UseDefaultFont { get; set; }
		public float BestFontSize { get; set; }
		public float BestFontTracking { get; set; }
		public float BestFontLineHeight { get; set; }

		public bool Italic
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.Italic); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.Italic);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.Italic);
				RaisePropertyChanged("Italic");
			}
		}
		public bool Bold
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.Bold); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.Bold);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.Bold);
				RaisePropertyChanged("Bold");
			}
		}
		public bool Underline
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.Underline); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.Underline);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.Underline);
				RaisePropertyChanged("Underline");
			}
		}
		public bool AddShadow
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.AddShadow); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.AddShadow);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.AddShadow);
				RaisePropertyChanged("AddShadow");
			}
		}
	}
}