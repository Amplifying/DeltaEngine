using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering2D.Graphs;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Models;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.Rendering3D.Shapes3D;
using GalaSoft.MvvmLight;

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

		private void SetUpStartEmitterData()
		{
			EmitterData = new ParticleEmitterData
			{
				SpawnInterval = 0.01f,
				LifeTime = 1,
				StartVelocity = new RangeGraph<Vector3D>(new Vector2D(0, -0.3f), new Vector2D(0, -0.3f)),
				Size = new RangeGraph<Size>(new Size(0.01f, 0.01f), new Size(0, 0)),
				Color = new RangeGraph<Color>(Datatypes.Color.White, Datatypes.Color.White),
				MaximumNumberOfParticles = 500
			};
			SelectedBillBoardMode = "Standard2D";
			RaisePropertyChanged("EmitterCreator");
			RaisePropertyChanged("SelectedBillBoardMode");
		}

		public ParticleEmitterData EmitterData { get; set; }

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

		private void Create2DEmitter()
		{
			if (EmitterData.ParticleMaterial == null ||
				(EmitterData.ParticleMaterial.Shader as ShaderWithFormat).Format.Is3D)
				return;
			emitter = new Particle2DEmitter(EmitterData,
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
			camera = Camera.Use<LookAtCamera>();
			camera.Position = Vector3D.One * 2.0f;
		}

		private Camera camera;

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

		public ParticleEmitter emitter;

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

		private Vector3D emitterPosition;

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
				MaterialList.Add(material);
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
			get { return selectedEmitter; }
			set
			{
				if (String.IsNullOrEmpty(value))
					return;
				selectedEmitter = value;
				RaisePropertyChanged("SelectedEmitter");
				if (emitter != null)
					emitter.IsActive = false;
				if (EmitterData.ParticleMaterial == null ||
					!(EmitterData.ParticleMaterial.Shader as ShaderWithFormat).Format.Is3D)
					return;
				CreateEmitter();
			}
		}

		private string selectedEmitter;

		public int MaxNumbersOfParticles
		{
			get { return EmitterData.MaximumNumberOfParticles; }
			set
			{
				if (emitter != null)
					emitter.IsActive = false;
				EmitterData.MaximumNumberOfParticles = value;
				CreateEmitter();
			}
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
		}

		private void SendSuccessMessageToLogger(ContentType type, string content)
		{
			Logger.Info("The saving of the particleData called " + particleName + " was a succes.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
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

		public RangeGraph<Vector3D> Acceleration
		{
			get { return EmitterData.Acceleration; }
			set
			{
				if (value != null)
					EmitterData.Acceleration = value;
			}
		}

		public RangeGraph<Vector3D> StartVelocity
		{
			get { return EmitterData.StartVelocity; }
			set
			{
				if (value != null)
					EmitterData.StartVelocity = value;
			}
		}

		public RangeGraph<Color> Color
		{
			get { return EmitterData.Color; }
			set
			{
				if (value != null)
					EmitterData.Color = value;
			}
		}

		public RangeGraph<ValueRange> RotationSpeed
		{
			get { return EmitterData.RotationSpeed; }
			set
			{
				if (value != null)
					EmitterData.RotationSpeed = value;
			}
		}

		public RangeGraph<ValueRange> StartRotation
		{
			get { return EmitterData.StartRotation; }
			set
			{
				if (value != null)
					EmitterData.StartRotation = value;
			}
		}

		public RangeGraph<Vector3D> StartPosition
		{
			get { return EmitterData.StartPosition; }
			set
			{
				if (value != null)
					EmitterData.StartPosition = value;
			}
		}

		public RangeGraph<Size> Size
		{
			get { return EmitterData.Size; }
			set
			{
				if (value != null)
					EmitterData.Size = value;
			}
		}

		public float LifeTime
		{
			get { return EmitterData.LifeTime; }
			set
			{
				EmitterData.LifeTime = value;
				RaisePropertyChanged("LifeTime");
			}
		}

		public float SpawnInterval
		{
			get { return EmitterData.SpawnInterval; }
			set
			{
				if (value != 0)
					EmitterData.SpawnInterval = value;
				RaisePropertyChanged("SpawnInterval");
			}
		}
	}
}