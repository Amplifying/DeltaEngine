using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Particles;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class ParticleEditorViewModel : ViewModelBase
	{
		public ParticleEditorViewModel(Service service)
		{
			ParticleList = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			SetUpStartEmitterData();
			this.service = service;
			metaDataCreator = new ContentMetaDataCreator(service);
			position = new Point(0.5f, 0.5f);
			CreateParticle();
			GetParticles();
			GetMaterials();
		}

		public ObservableCollection<string> ParticleList { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		private readonly Service service;
		private readonly ContentMetaDataCreator metaDataCreator;

		private void SetUpStartEmitterData()
		{
			EmitterCreator = new ParticleEmitterCreator
			{
				SpawnInterval = 0.01f,
				LifeTime = 1,
				Force = new Point(0, 0),
				StartVelocity = new Point(0, -0.3f),
				StartVelocityVariance = new Point(0.1f, 0),
				Size = new Range<Size>(new Size(0.01f, 0.01f), new Size(0, 0)),
				MaximumNumberOfParticles = 500,
				StartColor = Color.White,
				StartRotation = 0,
				RotationForce = 0
			};
			RaisePropertyChanged("EmitterCreator");
		}

		public ParticleEmitterCreator EmitterCreator { get; set; }

		private void CreateParticle()
		{
			EntitiesRunner.Current.Clear();
			if (EmitterCreator.ParticleMaterial == null)
				return;
			emitter = new ParticleEmitter(EmitterCreator, position);
		}

		public ParticleEmitter emitter;

		public Point Position
		{
			get { return position; }
			set
			{
				if (emitter != null)
					emitter.IsActive = false;
				position = value;
				CreateParticle();
			}
		}

		private Point position;

		private void GetParticles()
		{
			ParticleList.Clear();
			var foundParticles = service.GetAllContentNamesByType(ContentType.ParticleEffect);
			foreach (var particle in foundParticles)
				ParticleList.Add(particle);
		}

		private void GetMaterials()
		{
			var materialList = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in materialList)
				MaterialList.Add(material);
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
				EmitterCreator.ParticleMaterial = ContentLoader.Load<Material>(value);
				RaisePropertyChanged("EmitterData");
				CreateParticle();
			}
		}

		private string selectedMaterial;

		public int MaxNumbersOfParticles
		{
			get { return EmitterCreator.MaximumNumberOfParticles; }
			set
			{
				if (emitter != null)
					emitter.IsActive = false;
				EmitterCreator.MaximumNumberOfParticles = value;
				CreateParticle();
			}
		}

		public void Save()
		{
			var emitterData = FIllEmitterData();
			var bytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(emitterData);
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
		}

		public string ParticleName
		{
			get { return particleName; }
			set
			{
				particleName = value;
				if (ContentLoader.Exists(ParticleName, ContentType.ParticleEffect))
					EmitterCreator = ContentLoader.Load<ParticleEmitterCreator>(particleName);
				CreateParticle();
				RaisePropertyChanged("EmitterData");
				RaisePropertyChanged("SelectedMaterial");
			}
		}

		private string particleName;

		private ParticleEmitterData FIllEmitterData()
		{
			var emitterData = new ParticleEmitterData();
			emitterData.SpawnInterval = EmitterCreator.SpawnInterval;
			emitterData.Force = EmitterCreator.Force;
			emitterData.LifeTime = EmitterCreator.LifeTime;
			emitterData.MaximumNumberOfParticles = EmitterCreator.MaximumNumberOfParticles;
			emitterData.ParticleMaterial = EmitterCreator.ParticleMaterial;
			emitterData.Size = EmitterCreator.Size;
			emitterData.StartColor = EmitterCreator.StartColor;
			emitterData.StartRotation = EmitterCreator.StartRotation;
			emitterData.StartVelocity = EmitterCreator.StartVelocity;
			emitterData.StartVelocityVariance = EmitterCreator.StartVelocityVariance;
			return emitterData;
		}
	}
}