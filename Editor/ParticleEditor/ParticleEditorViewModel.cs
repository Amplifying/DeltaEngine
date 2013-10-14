using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Logging;
using DeltaEngine.Rendering2D.Graphs;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.Rendering3D.Shapes3D;
using GalaSoft.MvvmLight;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class ParticleEditorViewModel : ViewModelBase
	{
		public ParticleEditorViewModel(Service service)
		{
			EntitiesRunner.Current.Clear();
			ParticleList = new ObservableCollection<string>();
			EmitterList = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			BillBoardModeList = new ObservableCollection<string>();
			SetUpStartEmitterData();
			this.service = service;
			metaDataCreator = new ContentMetaDataCreator();
			emitterPosition = new Vector2D(0.5f, 0.5f);
			LifeTime = 1;
			camera = Camera.Use<LookAtCamera>();
			camera.Position = Vector3D.One * 2.0f;
			CreateEmitter();
			GetParticles();
			GetEmitterList();
			GetMaterials();
			GetBillboardModes();
		}

		public ObservableCollection<string> ParticleList { get; set; }
		public ObservableCollection<string> EmitterList { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		public ObservableCollection<string> BillBoardModeList { get; set; }
		private readonly Service service;
		private readonly ContentMetaDataCreator metaDataCreator;
		private Vector3D emitterPosition;
		private readonly TargetedCamera camera;

		public float LifeTime
		{
			get { return EmitterData.LifeTime; }
			set
			{
				if (value >= 0)
					EmitterData.LifeTime = value;
				RaisePropertyChanged("LifeTime");
			}
		}

		public ParticleEmitterData EmitterData { get; set; }

		private void SetUpStartEmitterData()
		{
			EmitterData = new ParticleEmitterData();
			EmitterData.SpawnInterval = 0.01f;
			EmitterData.LifeTime = 1;
			EmitterData.StartVelocity = new RangeGraph<Vector3D>(new Vector2D(0, -0.3f),
				new Vector2D(0, -0.3f));
			EmitterData.StartPosition = new RangeGraph<Vector3D>(new Vector3D(0, 0, 0),
				new Vector3D(0, 0, 0));
			EmitterData.Size = new RangeGraph<Size>(new Size(0.01f, 0.01f), new Size(0, 0));
			EmitterData.Color = new RangeGraph<Color>(Datatypes.Color.White, Datatypes.Color.White);
			EmitterData.MaximumNumberOfParticles = 500;
			SelectedBillBoardMode = "Standard2D";
			RaisePropertyChanged("EmitterCreator");
			RaisePropertyChanged("SelectedBillBoardMode");
		}

		public string SelectedBillBoardMode
		{
			get { return selectedBillBoardMode; }
			set
			{
				try
				{
					selectedBillBoardMode = value;
					EmitterData.BillboardMode =
						(BillboardMode)Enum.Parse(typeof(BillboardMode), selectedBillBoardMode);
				}
				catch
				{
					EmitterData.BillboardMode = BillboardMode.CameraFacing;
					selectedBillBoardMode = EmitterData.BillboardMode.ToString();
				}
				CreateEmitter();
			}
		}

		private string selectedBillBoardMode;

		private void CreateEmitter()
		{
			EntitiesRunner.Current.Clear();
			if (emitter != null)
				emitter.IsActive = false;
			if (EmitterData.BillboardMode == BillboardMode.Standard2D)
				Create2DEmitter();
			else
				Create3DEmitter();
		}

		public ParticleEmitter emitter;

		private void Create2DEmitter()
		{
			if (EmitterData.ParticleMaterial == null ||
				(EmitterData.ParticleMaterial.Shader as ShaderWithFormat).Format.Is3D)
				return;
			emitter = new ParticleEmitter(EmitterData,
				new Vector2D(emitterPosition.X, emitterPosition.Y));
		}

		private void Create3DEmitter()
		{
			SetGridAndCameraTo3D();
			ResetEmitter3D();
		}

		private void SetGridAndCameraTo3D()
		{
			new Grid3D(10);
			SetCommands();
		}

		private void SetCommands()
		{
			new Command(FireParticleBurst).Add(new KeyTrigger(Key.Space));
			new Command(() => GetCameraMovedAroundZ(1.0f)).Add(new KeyTrigger(Key.D, State.Pressed));
			new Command(() => GetCameraMovedAroundZ(-1.0f)).Add(new KeyTrigger(Key.A, State.Pressed));
			new Command(() => GetCameraMovedAroundObject(1.0f)).Add(new KeyTrigger(Key.W, State.Pressed));
			new Command(() => GetCameraMovedAroundObject(-1.0f)).Add(new KeyTrigger(Key.S, State.Pressed));
		}

		private void FireParticleBurst()
		{
			CreateEmitter();
			if (emitter == null || EmitterData.BillboardMode == BillboardMode.Standard2D)
				return;
			emitter.DisposeAfterSeconds(EmitterData.LifeTime);
		}

		private void GetCameraMovedAroundZ(float multiplier)
		{
			var front = camera.Target - camera.Position;
			front.Normalize();
			var right = Vector3D.Cross(front, Vector3D.UnitZ);
			camera.Position += right * Time.Delta * 2 * multiplier;
		}

		private void GetCameraMovedAroundObject(float multiplier)
		{
			var front = camera.Target - camera.Position;
			front.Normalize();
			var right = Vector3D.Cross(front, Vector3D.UnitZ);
			var up = Vector3D.Cross(right, front);
			camera.Position += up * Time.Delta * 2 * multiplier;
		}

		private void ResetEmitter3D()
		{
			if (EmitterData.ParticleMaterial == null ||
				!(EmitterData.ParticleMaterial.Shader as ShaderWithFormat).Format.Is3D)
				return;
			if (EmitterData.EmitterType == "PointEmitter")
				emitter = new Particle3DPointEmitter(EmitterData, EmitterData.StartPosition.Start);
			else if (EmitterData.EmitterType == "LineEmitter")
				emitter = new Particle3DLineEmitter(EmitterData, EmitterData.StartPosition);
			else if (EmitterData.EmitterType == "BoxEmitter")
				emitter = new Particle3DBoxEmitter(EmitterData, EmitterData.StartPosition);
			else if (EmitterData.EmitterType == "SphericalEmitter")
				emitter = new Particle3DSphericalEmitter(EmitterData, EmitterData.StartPosition.Start,
					EmitterData.StartPosition.End.Length);
		}

		private void GetParticles()
		{
			ParticleList.Clear();
			var foundParticles = service.GetAllContentNamesByType(ContentType.ParticleEmitter);
			foreach (var particle in foundParticles)
				ParticleList.Add(particle);
		}

		private void GetEmitterList()
		{
			EmitterList.Add("PointEmitter");
			EmitterList.Add("LineEmitter");
			EmitterList.Add("BoxEmitter");
			EmitterList.Add("SphericalEmitter");
		}

		private void GetMaterials()
		{
			var materialList = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in materialList)
				if (HasMaterial(material))
					MaterialList.Add(material);
		}

		private static bool HasMaterial(string material)
		{
			try
			{
				return (ContentLoader.Load<Material>(material).Shader as ShaderWithFormat).Format.HasColor;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void GetBillboardModes()
		{
			BillBoardModeList.Clear();
			var newModeList = Enum.GetValues(typeof(BillboardMode));
			foreach (var mode in newModeList)
				BillBoardModeList.Add(((BillboardMode)mode).ToString());
			if (!BillBoardModeList.Contains(selectedBillBoardMode))
				selectedBillBoardMode = BillBoardModeList[0];
		}

		public void Save()
		{
			if (string.IsNullOrEmpty(particleName))
			{
				Logger.Info("Saving of content data requires a name, cannot save with an empty name.");
				return;
			}
			var bytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(EmitterData);
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add(ParticleName + ".deltaparticle", bytes);
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromParticle(ParticleName,
				bytes);
			if (ContentLoader.Exists(ParticleName))
			{
				service.DeleteContent(ParticleName);
				ContentLoader.RemoveResource(ParticleName);
			}
			service.UploadContent(contentMetaData, fileNameAndBytes);
			service.ContentUpdated += SendSuccessMessageToLogger;
			ReloadParticleValues();
			CreateEmitter();
		}

		public string ParticleName
		{
			get { return particleName; }
			set
			{
				particleName = value;
				if (ContentLoader.Exists(ParticleName, ContentType.ParticleEmitter))
					EmitterData = ContentLoader.Load<ParticleEmitterData>(particleName);
				ReloadParticleValues();
				CreateEmitter();
				RaisePropertiesChanged();
			}
		}

		private string particleName;

		private void ReloadParticleValues()
		{
			UpdateProperties();
			RaisePropertiesChanged();
		}

		private void UpdateProperties()
		{
			if (EmitterData.ParticleMaterial == null)
				return;
			SelectedMaterial = EmitterData.ParticleMaterial.Name;
			SelectedEmitter = EmitterData.EmitterType;
			Size = EmitterData.Size;
			Color = EmitterData.Color;
			LifeTime = EmitterData.LifeTime;
			SelectedBillBoardMode = EmitterData.BillboardMode.ToString();
			StartPosition = EmitterData.StartPosition;
			StartRotation = EmitterData.StartRotation;
			StartVelocity = EmitterData.StartVelocity;
			Acceleration = EmitterData.Acceleration;
			RotationSpeed = EmitterData.RotationSpeed;
			SpawnInterval = EmitterData.SpawnInterval;
			MaxNumbersOfParticles = EmitterData.MaximumNumberOfParticles;
		}

		public string SelectedMaterial
		{
			get { return selectedMaterial; }
			set
			{
				if (String.IsNullOrEmpty(value))
					return;
				selectedMaterial = value;
				RaisePropertyChanged("SelectedMaterial");
				if (emitter != null)
					emitter.IsActive = false;
				EmitterData.ParticleMaterial = ContentLoader.Load<Material>(value);
				RaisePropertyChanged("EmitterData");
				CreateEmitter();
			}
		}

		private string selectedMaterial;

		public string SelectedEmitter
		{
			get { return EmitterData.EmitterType; }
			set
			{
				if (String.IsNullOrEmpty(value))
					return;
				EmitterData.EmitterType = value;
				RaisePropertyChanged("SelectedEmitter");
				if (emitter != null)
					emitter.IsActive = false;
				if (EmitterData.ParticleMaterial == null ||
					!(EmitterData.ParticleMaterial.Shader as ShaderWithFormat).Format.Is3D)
					return;
				CreateEmitter();
			}
		}

		public RangeGraph<Size> Size
		{
			get { return EmitterData.Size; }
			set
			{
				if (value != null)
					EmitterData.Size = value;
				CreateEmitter();
			}
		}

		public RangeGraph<Color> Color
		{
			get { return EmitterData.Color; }
			set
			{
				if (value != null)
					EmitterData.Color = value;
				CreateEmitter();
			}
		}

		public RangeGraph<Vector3D> StartPosition
		{
			get { return EmitterData.StartPosition; }
			set
			{
				if (value == null)
					return;
				EmitterData.StartPosition = value;
				CreateEmitter();
			}
		}

		public RangeGraph<ValueRange> StartRotation
		{
			get { return EmitterData.StartRotation; }
			set
			{
				if (value == null)
					return;
				EmitterData.StartRotation = value;
				CreateEmitter();
			}
		}

		public RangeGraph<Vector3D> StartVelocity
		{
			get { return EmitterData.StartVelocity; }
			set
			{
				if (value != null)
					EmitterData.StartVelocity = value;
				CreateEmitter();
			}
		}

		public RangeGraph<Vector3D> Acceleration
		{
			get { return EmitterData.Acceleration; }
			set
			{
				if (value != null)
					EmitterData.Acceleration = value;
				CreateEmitter();
			}
		}

		public RangeGraph<ValueRange> RotationSpeed
		{
			get { return EmitterData.RotationSpeed; }
			set
			{
				if (value == null)
					return;
				EmitterData.RotationSpeed = value;
				CreateEmitter();
			}
		}

		public float SpawnInterval
		{
			get { return EmitterData.SpawnInterval; }
			set
			{
				EmitterData.SpawnInterval = value;
				RaisePropertyChanged("SpawnInterval");
				CreateEmitter();
			}
		}

		public int MaxNumbersOfParticles
		{
			get { return EmitterData.MaximumNumberOfParticles; }
			set
			{
				if (emitter != null)
					emitter.IsActive = false;
				EmitterData.MaximumNumberOfParticles = SetNumberOfParticles(value);
				CreateEmitter();
			}
		}

		private static int SetNumberOfParticles(int value)
		{
			if (value <= 1024 && value >= 0)
				return value;
			Logger.Info("Maximum number of particles is 1024 and Minimum 0");
			return value > 1024 ? 1024 : 0;
		}

		public TextLogger TextLogger { get; set; }

		private void RaisePropertiesChanged()
		{
			RaisePropertyChanged("EmitterData");
			RaisePropertyChanged("SelectedMaterial");
			RaisePropertyChanged("SelectedEmitter");
			RaisePropertyChanged("Size");
			RaisePropertyChanged("Color");
			RaisePropertyChanged("LifeTime");
			RaisePropertyChanged("SelectedBillBoardMode");
			RaisePropertyChanged("StartPosition");
			RaisePropertyChanged("StartRotation");
			RaisePropertyChanged("StartVelocity");
			RaisePropertyChanged("Acceleration");
			RaisePropertyChanged("RotationSpeed");
			RaisePropertyChanged("SpawnInterval");
			RaisePropertyChanged("MaxNumberOfParticles");
		}

		private void SendSuccessMessageToLogger(ContentType type, string content)
		{
			Logger.Info("The saving of the particleData called " + particleName + " was a succes.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
		}

		public void SwitchGradientGraph()
		{
			if (currentGraph != null)
				return;
			currentGraph = new GradientGraph(GetGraphDrawArea(), EmitterData.Color);
		}

		private GradientGraph currentGraph;

		private static Rectangle GetGraphDrawArea()
		{
			return new Rectangle(0.1f, 0.6f, 0.8f, 0.1f);
		}

		public Vector3D EmitterPosition
		{
			get { return emitterPosition; }
			set
			{
				if (emitter != null)
					emitter.IsActive = false;
				emitterPosition = value;
				CreateEmitter();
			}
		}

		public void DeleteParticleContent()
		{
			service.DeleteContent(particleName);
			Logger.Info("Particle content " + particleName + " has been deleted.");
			SetUpStartEmitterData();
			camera.Position = Vector3D.One * 2.0f;
			LifeTime = 1;
			ParticleName = "";
		}
	}
}