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
using DeltaEngine.Logging;
using DeltaEngine.Rendering2D.Particles;
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
		}

		public ObservableCollection<string> ParticleList { get; set; }
		public ObservableCollection<string> EmitterList { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		public ObservableCollection<string> BillBoardModeList { get; set; }
		private readonly Service service;
		private readonly ContentMetaDataCreator metaDataCreator;
		private Vector3D emitterPosition;

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
			RaisePropertyChanged("EmitterCreator");
			RaisePropertyChanged("SelectedBillBoardMode");
		}

		private void CreateEmitter()
		{
			EntitiesRunner.Current.Clear();
			if (emitter != null)
				emitter.IsActive = false;
			Create2DEmitter();
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
			SaveParticleSystemData(particleName + "System", new[] { particleName });
			ReloadParticleValues();
			CreateEmitter();
		}

		private void SaveParticleSystemData(string particleSystemName, string[] emitterNames)
		{
			var metaData = metaDataCreator.CreateParticleSystemData(particleSystemName, emitterNames);
			if (ContentLoader.Exists(particleSystemName))
			{
				service.DeleteContent(particleSystemName);
				ContentLoader.RemoveResource(particleSystemName);
			}
			service.UploadContent(metaData);
			service.ContentUpdated += SendSuccessMessageToLogger;
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
			LifeTime = 1;
			ParticleName = "";
		}
	}
}