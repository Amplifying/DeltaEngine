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
using DeltaEngine.Rendering.Particles;
using DeltaEngine.Rendering.Sprites;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class ParticleEditorViewModel : ViewModelBase
	{
		public ParticleEditorViewModel(Service service)
		{
			ParticleList = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			BillBoardModeList = new ObservableCollection<string>();
			SetUpStartEmitterData();
			this.service = service;
			metaDataCreator = new ContentMetaDataCreator(service);
			emitterPosition = new Point(0.5f, 0.5f);
			CreateEmitter();
			GetParticles();
			GetMaterials();
			GetBillboardModes();
		}

		public ObservableCollection<string> ParticleList { get; set; }
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
				StartVelocity = new RangeGraph<Vector>(new Point(0, -0.3f), new Point(0, -0.3f)),
				Size = new RangeGraph<Size>(new Size(0.01f, 0.01f), new Size(0, 0)),
				MaximumNumberOfParticles = 500
			};
			SelectedBillBoardMode = "Standard2D";
			RaisePropertyChanged("EmitterCreator");
			RaisePropertyChanged("SelectedBillBoardMode");
		}

		public ParticleEmitterData EmitterData { get; set; }

		private void CreateEmitter()
		{
			if (EmitterData.BillboardMode == BillboardMode.Standard2D)
				Create2DEmitter();
			else
				Create3DEmitter();
		}

		private void Create2DEmitter()
		{
			EntitiesRunner.Current.Clear();
			if (EmitterData.ParticleMaterial == null)
				return;
			emitter = new Particle2DEmitter(EmitterData, new Point(emitterPosition.X, emitterPosition.Y));
		}

		private void Create3DEmitter()
		{
			EntitiesRunner.Current.Clear();
			if (EmitterData.ParticleMaterial == null)
				return;
			emitter = new Particle3DPointEmitter(EmitterData, Vector.Zero);
		}

		public ParticleEmitter emitter;

		public Vector EmitterPosition
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

		private Vector emitterPosition;

		private void GetParticles()
		{
			ParticleList.Clear();
			var foundParticles = service.GetAllContentNamesByType(ContentType.Particle2DEmitter);
			foreach (var particle in foundParticles)
				ParticleList.Add(particle);
		}

		private void GetMaterials()
		{
			var materialList = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in materialList)
				MaterialList.Add(material);
		}

		private void GetBillboardModes()
		{
			var newModeList = new ObservableCollection<string>();
			var billBoardModes = Enum.GetValues(typeof(BillboardMode));
			foreach (var billboardMode in billBoardModes)
				AddBillboardModesFittingShaders((BillboardMode)billboardMode, ref newModeList);
			BillBoardModeList = newModeList;
			if (!BillBoardModeList.Contains(selectedBillBoardMode))
				selectedBillBoardMode = BillBoardModeList[0];
		}

		private void AddBillboardModesFittingShaders(BillboardMode billboardMode,
			ref ObservableCollection<string> modeList)
		{
			if (EmitterData.ParticleMaterial == null)
			{
				modeList.Add(billboardMode.ToString());
				return;
			}
			var shaderFormat = (EmitterData.ParticleMaterial.Shader as ShaderWithFormat).Format;
			if (shaderFormat.Is3D)
				if (billboardMode != BillboardMode.Standard2D)
					modeList.Add(billboardMode.ToString());
				else if (billboardMode == BillboardMode.Standard2D)
					modeList.Add(billboardMode.ToString());
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
				GetBillboardModes();
				RaisePropertyChanged("SelectedMaterial");
				if (emitter != null)
					emitter.IsActive = false;
				EmitterData.ParticleMaterial = ContentLoader.Load<Material>(value);
				RaisePropertyChanged("EmitterData");
				CreateEmitter();
			}
		}

		private string selectedMaterial;

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
			Logger.Info("The saving of the animation called " + particleName + " was a succes.");
			service.ContentUpdated -= SendSuccessMessageToLogger;
		}

		public string ParticleName
		{
			get { return particleName; }
			set
			{
				particleName = value;
				if (ContentLoader.Exists(ParticleName, ContentType.Particle2DEmitter))
					EmitterData = ContentLoader.Load<ParticleEmitterData>(particleName);
				CreateEmitter();
				RaisePropertyChanged("EmitterData");
				RaisePropertyChanged("SelectedMaterial");
			}
		}

		private string particleName;
	}
}