using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D.Shapes.Levels;
using GalaSoft.MvvmLight;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.TileEditor
{
	public sealed class TileEditorViewModel : ViewModelBase
	{
		public TileEditorViewModel()
		{
			ClearEntities();
			Map = new Level(new Size(8, 8));
			Waves = new ObservableCollection<Wave>();
			RaisePropertyChanged("Map");
			renderer = new LevelRenderer(Map);
			new Command(Command.Click, position => Map.SetTile(position, SelectedTileType));
			new Command(Command.RightClick, position => Map.SetTile(position, LevelTileType.Nothing));
		}

		private LevelRenderer renderer;

		private static void ClearEntities()
		{
			if (EntitiesRunner.Current == null)
				return;
			foreach (var entity in EntitiesRunner.Current.GetAllEntities())
				entity.IsActive = false;
			EntitiesRunner.Current.Clear();

		}

		public TileEditorViewModel(Service service)
			: this()
		{
			this.service = service;
		}

		private readonly Service service;

		public void OpenXmlFile()
		{
			var filename = Path.Combine(Directory.GetCurrentDirectory(), "Content", service.ProjectName,
				ContentName + ".xml");
			if (File.Exists(filename))
				Process.Start(filename);
			else
				MessageBox.Show("Cannot open xml file because the level was not saved yet!");
		}

		public string ContentName
		{
			get { return contentName; }
			set
			{
				contentName = value;
				CreateTileMapFromContent();
			}
		}

		private string contentName;

		private void CreateTileMapFromContent()
		{
			if (!ContentLoader.Exists(contentName, ContentType.Level))
				return;
			renderer.Dispose();
			ClearEntities();
			Map = ContentLoader.Load<Level>(contentName);
			RaisePropertyChanged("Map");
			renderer = new LevelRenderer(Map);
		}

		public string CustomSize
		{
			get { return Map.Size.ToString(); }
			set
			{
				if (value.SplitAndTrim(',').Length != 2)
					return;
				Map.Size = new Size(value);
				RaisePropertyChanged("Map");
			}
		}

		public Level Map { get; private set; }
		private Wave selectedWave;
		public LevelTileType SelectedTileType { get; set; }
		public float WaitTime { get; set; }
		public float SpawnInterval { get; set; }
		public float MaxTime { get; set; }
		public string SpawnTypeList { get; set; }

		public Wave SelectedWave
		{
			get { return selectedWave; }
			set
			{
				selectedWave = value;
				if (selectedWave == null)
					return;
				WaitTime = selectedWave.WaitTime;
				SpawnInterval = selectedWave.SpawnInterval;
				MaxTime = selectedWave.MaxTime;
				SpawnTypeList = selectedWave.SpawnTypeList.ToText();
				RaisePropertyChanged("WaitTime");
				RaisePropertyChanged("SpawnInterval");
				RaisePropertyChanged("MaxTime");
				RaisePropertyChanged("SpawnTypeList");
			}
		}

		public ObservableCollection<Wave> Waves { get; set; }

		public bool IsWaveSelected
		{
			get { return SelectedWave != null; }
		}

		public void SaveToServer()
		{
			var metaDataToSend = new ContentMetaData
			{
				Name = ContentName,
				Type = ContentType.Level,
				LocalFilePath = ContentName + ".xml"
			};
			var data = new XmlFile(BuildXmlData()).ToMemoryStream().ToArray();
			var dataAndName = new Dictionary<string, byte[]> { { ContentName + ".xml", data } };
			service.UploadContent(metaDataToSend, dataAndName);
		}

		public XmlData BuildXmlData()
		{
			var xml = new XmlData("Level");
			xml.AddAttribute("Name", ContentName);
			xml.AddAttribute("Size", Map.Size);
			AddPoints(xml, LevelTileType.SpawnPoint);
			AddPoints(xml, LevelTileType.ExitPoint);
			xml.AddChild("Map", Map.ToTextForXml());
			AddWaves(xml);
			return xml;
		}

		private void AddPoints(XmlData xml, LevelTileType pointType)
		{
			int counter = 0;
			foreach (var point in FindPoints(pointType))
			{
				var pointXml = new XmlData(pointType.ToString());
				pointXml.AddAttribute("Name", pointType.ToString().Replace("Point", "") + (counter++));
				pointXml.AddAttribute("Position", point);
				xml.AddChild(pointXml);
			}
		}

		private IEnumerable<Point> FindPoints(LevelTileType pointType)
		{
			var result = new List<Point>();
			for (int y = 0; y < Map.Size.Height; y++)
				for (int x = 0; x < Map.Size.Width; x++)
					if (Map.MapData[x, y] == pointType)
						result.Add(new Point(x, y));
			return result;
		}

		private void AddWaves(XmlData xml)
		{
			foreach (var wave in Waves)
				xml.AddChild(wave.AsXmlData());
		}

		public void PlaceTile(LevelTileType tileType, Point position)
		{
			Map.MapData[(int)position.X, (int)position.Y] = tileType;
		}

		public void AddWave(float waitTime, float spawnInterval, float maxTime, string thingsToSpawn)
		{
			Waves.Add(new Wave(waitTime, spawnInterval, maxTime, thingsToSpawn));
		}

		public void RemoveSelectedWave()
		{
			Waves.Remove(selectedWave);
		}
	}
}