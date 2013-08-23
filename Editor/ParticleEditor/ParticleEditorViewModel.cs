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
			EmitterData = new ParticleEffectData
			{
				SpawnInterval = 0.01f,
				LifeTime = 1,
				StartVelocity = new RangeGraph<Point>(new Point(0, -0.3f), new Point(0, -0.3f)),
				Size = new RangeGraph<Size>(new Size(0.01f, 0.01f), new Size(0, 0)),
				MaximumNumberOfParticles = 500
			};
			RaisePropertyChanged("EmitterCreator");
		}

		public ParticleEffectData EmitterData { get; set; }

		private void CreateParticle()
		{
			EntitiesRunner.Current.Clear();
			if (EmitterData.ParticleMaterial == null)
				return;
			emitter = new ParticleEmitter(EmitterData, position);
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
				EmitterData.ParticleMaterial = ContentLoader.Load<Material>(value);
				RaisePropertyChanged("EmitterData");
				CreateParticle();
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
				CreateParticle();
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
		}

		public string ParticleName
		{
			get { return particleName; }
			set
			{
				particleName = value;
				if (ContentLoader.Exists(ParticleName, ContentType.ParticleEffect))
					EmitterData = ContentLoader.Load<ParticleEffectData>(particleName);
				CreateParticle();
				RaisePropertyChanged("EmitterData");
				RaisePropertyChanged("SelectedMaterial");
			}
		}

		private string particleName;
	}
}