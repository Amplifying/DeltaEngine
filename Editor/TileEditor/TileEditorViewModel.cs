using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.TileEditor
{
	public class TileEditorViewModel : ViewModelBase
	{
		public TileEditorViewModel()
		{
		}

		public TileEditorViewModel(Service service)
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

		public string ContentName { get; set; }

		public Size Size
		{
			get { return size; }
			set
			{
				size = value;
				Map = new TileType[(int)size.Width, (int)size.Height];
			}
		}

		private Size size;
		public TileType SelectedTileType { get; set; }
		public TileType[,] Map { get; private set; }
		private readonly List<Wave> waves = new List<Wave>();

		public void SaveToServer()
		{
			var metaDataToSend = new ContentMetaData { Name = ContentName, Type = ContentType.Level };
			var data = new XmlFile(BuildXmlData()).ToMemoryStream().ToArray();
			var dataAndName = new Dictionary<string, byte[]> { { ContentName + ".xml", data } };
			service.UploadContent(metaDataToSend, dataAndName);
		}

		public XmlData BuildXmlData()
		{
			var xml = new XmlData("Level");
			xml.AddAttribute("Name", ContentName);
			xml.AddAttribute("Size", Size);
			AddPoints(xml, TileType.SpawnPoint);
			AddPoints(xml, TileType.GoalPoint);
			xml.AddChild("Map", MapAsTextForXml());
			AddWaves(xml);
			return xml;
		}

		private void AddPoints(XmlData xml, TileType pointType)
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

		private IEnumerable<Point> FindPoints(TileType pointType)
		{
			var result = new List<Point>();
			for (int y = 0; y < Size.Height; y++)
				for (int x = 0; x < Size.Width; x++)
					if (Map[x, y] == pointType)
						result.Add(new Point(x, y));
			return result;
		}

		private string MapAsTextForXml()
		{
			string result = Environment.NewLine;
			for (int y = 0; y < Size.Height; y++)
			{
				for (int x = 0; x < Size.Width; x++)
					result += LetterForTileType(Map[x, y]);
				result += Environment.NewLine;
			}
			return result;
		}

		private static char LetterForTileType(TileType tileType)
		{
			switch (tileType)
			{
			case TileType.Nothing:
				return '.';
			case TileType.Blocked:
				return 'X';
			case TileType.Placeable:
				return 'P';
			case TileType.BlockedPlaceable:
				return 'L';
			case TileType.Red:
				return 'R';
			case TileType.Green:
				return 'G';
			case TileType.Blue:
				return 'B';
			case TileType.Yellow:
				return 'Y';
			case TileType.Brown:
				return 'O';
			case TileType.Gray:
				return 'A';
			case TileType.SpawnPoint:
				return 'S';
			case TileType.GoalPoint:
				return 'G';
			default:
				throw new ArgumentOutOfRangeException("tileType");
			}
		}

		private void AddWaves(XmlData xml)
		{
			foreach (var wave in waves)
				xml.AddChild(wave.AsXmlData());
		}

		public void PlaceTile(TileType tileType, Point position)
		{
			throw new NotImplementedException();
		}

		public void AddWave(float waitTime, float spawnInterval, string thingsToSpawn)
		{
			throw new NotImplementedException();
		}
	}
}