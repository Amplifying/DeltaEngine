using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.GameLogic;
using DeltaEngine.Input;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes3D;
using GalaSoft.MvvmLight;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.LevelEditor
{
	public sealed class LevelEditorViewModel : ViewModelBase
	{
		public LevelEditorViewModel(Service service)
		{
			this.service = service;
			ClearEntities();
			Map = new Level(new Size(8, 8));
			Decals = new List<Billboard>();
			SetCamera();
			Map.RenderIn3D = true;
			RaisePropertyChanged("Map");
			Renderer = new LevelRenderer(Map);
			DecalMaterials = new ObservableCollection<string>();
			GetDecalMaterials();
			SetCommands();
		}

		public readonly List<Billboard> Decals;
		public ObservableCollection<string> DecalMaterials { get; set; }
		public LevelRenderer Renderer;

		private static void ClearEntities()
		{
			if (EntitiesRunner.Current == null)
				return;
			foreach (var entity in EntitiesRunner.Current.GetAllEntities())
				entity.IsActive = false;
			EntitiesRunner.Current.Clear();
		}

		private void GetDecalMaterials()
		{
			foreach (var material in service.GetAllContentNamesByType(ContentType.Material))
				DecalMaterials.Add(material);
		}

		private void SetCommands()
		{
			new Command(position =>
			{
				mousePosition = position;
				if (NotPlacingTile)
					SelectedDecal = GetDecalFromPosition(position);
				else
					Map.SetTileWithScreenPosition(position, SelectedTileType);
			}).Add(new MouseButtonTrigger());
			new Command(Command.RightClick,
				position =>
				{
					Map.SetTileWithScreenPosition(position, LevelTileType.Nothing);
				});
			new Command(Command.Drag, position =>
			{
				if (Is3D)
					return;
				var distance = mousePosition - position;
				Map.Position = Map.Position - distance;
				mousePosition = position;
				Renderer.UpdateLevel();
			});
			new Command(Command.Zoom, position =>
			{
				if (Is3D)
					return;
				Map.gridScaling += new Size(position / 25 * Map.gridScaling.Width);
				Renderer.UpdateLevel();
			});
			new Command(position => AddDecal(position)).Add(new MouseButtonTrigger(MouseButton.Left,
				State.Releasing));
		}

		private Vector2D mousePosition;
		private readonly Service service;

		public Billboard GetDecalFromPosition(Vector2D screenPosition)
		{
			var ray = Camera.Current.ScreenPointToRay(screenPosition);
			var floor = new Plane(Vector3D.UnitZ, 0.0f);
			var position = (Vector3D)floor.Intersect(ray);
			foreach (var decal in Decals)
				if (position.X > decal.Position.X &&
					position.X < decal.Position.X + decal.planeQuad.Size.Width &&
					position.Y > decal.Position.Y &&
					position.Y < decal.Position.Y + decal.planeQuad.Size.Height)
					return decal;
			return null;
		}

		private void AddDecal(Vector2D screenPosition)
		{
			if (!isDraggingDecal)
				return;
			var ray = Camera.Current.ScreenPointToRay(screenPosition);
			var floor = new Plane(Vector3D.UnitZ, 0.0f);
			var position = (Vector3D)floor.Intersect(ray);
			Decals.Add(new Billboard(position, new Size(0.55f),
				new Material(Shader.Position3DColorUV, "DefaultImage"), BillboardMode.Ground));
			isDraggingDecal = false;
		}

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
				RaisePropertyChanged("Waves");
			}
		}

		private string contentName;

		private void CreateTileMapFromContent()
		{
			if (!ContentLoader.Exists(contentName, ContentType.Level))
				return;
			Renderer.Dispose();
			ClearEntities();
			RaisePropertyChanged("Map");
			Map = ContentLoader.Load<Level>(contentName);
			SetCommands();
			CustomSize = Map.Size.ToString();
			Renderer = new LevelRenderer(Map);
		}

		public string CustomSize
		{
			get { return Map.Size.ToString(); }
			set
			{
				if (value.SplitAndTrim(',').Length != 2)
					return;
				Map.Size = new Size(value);
				Renderer = new LevelRenderer(Map);
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
			foreach (var wave in Map.Waves)
				xml.AddChild(wave.AsXmlData());
			RaisePropertyChanged("Waves");
		}

		public void PlaceTile(LevelTileType tileType, Point position)
		{
			Map.MapData[(int)position.X, (int)position.Y] = tileType;
		}

		public void AddWave(float waitTime, float spawnInterval, float maxTime, string thingsToSpawn)
		{
			Map.Waves.Add(new Wave(waitTime, spawnInterval, thingsToSpawn, maxTime));
			RaisePropertyChanged("Waves");
		}

		public void RemoveSelectedWave()
		{
			Map.Waves.Remove(selectedWave);
			RaisePropertyChanged("Waves");
		}

		public List<Wave> Waves
		{
			get { return Map.Waves; }
			set
			{
				Map.Waves = value;
				RaisePropertyChanged("Waves");
			}
		}

		public bool Is3D
		{
			get { return Map.RenderIn3D; }
			set
			{
				if (value && Camera.Current == null)
					SetCamera();
				Map.RenderIn3D = value;
				Renderer.Dispose();
				Renderer = new LevelRenderer(Map);
				RaisePropertyChanged("Waves");
			}
		}

		private static void SetCamera()
		{
			camera3D = Camera.Use<LookAtCamera>();
			camera3D.Position = 3 * Vector3D.One;
			camera3D.Target = Vector3D.Zero;
		}

		private static LookAtCamera camera3D;

		public void SetDragingDecal(bool draggingDecal)
		{
			isDraggingDecal = draggingDecal;
		}

		private bool isDraggingDecal;

		public string SelectedMaterial
		{
			get { return selectedMaterial; }
			set
			{
				selectedMaterial = value;
				if (selectedMaterial == null)
					return;
				SelectedDecal.planeQuad.Material = ContentLoader.Load<Material>(selectedMaterial);
			}
		}
		private string selectedMaterial;

		public bool NotPlacingTile { get; set; }

		public Billboard SelectedDecal
		{
			get { return selectedDecal; }
			set
			{
				selectedDecal = value;
				if (selectedDecal == null)
					return;
				SelectedMaterial = selectedDecal.planeQuad.Material.MetaData.Name;
				DecalSize = selectedDecal.planeQuad.Size;
			}
		}

		private Billboard selectedDecal;

		public Size DecalSize
		{
			get { return SelectedDecal == null ? new Size(0) : SelectedDecal.planeQuad.Size; }
			set
			{
				//foreach (var decal in Map.Decals)
				//	if (decal == SelectedDecal)
				//		decal.planeQuad.Size = value;
				SelectedDecal.planeQuad.Size = value;
				RaisePropertyChanged("DecalSize");
			}
		}
	}
}