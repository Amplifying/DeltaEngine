using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Platforms;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to load and play simple sound effects by creating a SoundInstance per play.
	/// </summary>
	public abstract class Sound : ContentData
	{
		protected Sound(string contentName, Settings settings)
			: base(contentName)
		{
			this.settings = settings;
		}

		private readonly Settings settings;

		public abstract float LengthInSeconds { get; }
		public int NumberOfInstances
		{
			get { return internalInstances.Count + externalInstances.Count; }
		}

		public int NumberOfPlayingInstances
		{
			get
			{
				return internalInstances.Count(instance => instance.IsPlaying) + 
					externalInstances.Count(instance => instance.IsPlaying);
			}
		}

		protected override void DisposeData()
		{
			foreach (var instance in internalInstances.ToList())
				Remove(instance);

			foreach (var instance in externalInstances.ToList())
				Remove(instance);
		}

		public void Play()
		{
			Play(settings.SoundVolume, 0.0f);
		}

		public void Play(float panning)
		{
			Play(settings.SoundVolume, panning);
		}

		public void Play(float volume, float panning)
		{
			SoundInstance freeInstance = GetInternalNonPlayingInstance();
			freeInstance.Volume = volume;
			freeInstance.Panning = panning;
			freeInstance.Play();
		}

		private SoundInstance GetInternalNonPlayingInstance()
		{
			SoundInstance freeInstance = internalInstances.FirstOrDefault(i => !IsPlaying(i));
			if (freeInstance != null)
				return freeInstance;
			createInternalInstance = true;
			return CreateSoundInstance();
		}

		private bool createInternalInstance;

		public SoundInstance CreateSoundInstance()
		{
			var instance = new SoundInstance(this) { Volume = settings.SoundVolume };
			Add(instance);

			return instance;
		}

		public abstract void PlayInstance(SoundInstance instanceToPlay);

		public void StopAll()
		{
			foreach (var instance in internalInstances)
				instance.Stop();

			foreach (var instance in externalInstances)
				instance.Stop();
		}

		public abstract void StopInstance(SoundInstance instanceToStop);

		protected abstract void CreateChannel(SoundInstance instanceToFill);
		protected abstract void RemoveChannel(SoundInstance instanceToRemove);

		private readonly List<SoundInstance> internalInstances = new List<SoundInstance>();
		private readonly List<SoundInstance> externalInstances = new List<SoundInstance>();

		internal void Add(SoundInstance instanceToAdd)
		{
			if (createInternalInstance)
				internalInstances.Add(instanceToAdd);
			else
				externalInstances.Add(instanceToAdd);

			createInternalInstance = false;
			CreateChannel(instanceToAdd);
		}

		internal void Remove(SoundInstance instanceToRemove)
		{
			internalInstances.Remove(instanceToRemove);
			externalInstances.Remove(instanceToRemove);
			RemoveChannel(instanceToRemove);
		}

		public abstract bool IsPlaying(SoundInstance instance);

		public bool IsAnyInstancePlaying
		{
			get { return internalInstances.Any(IsPlaying) || externalInstances.Any(IsPlaying); }
		}

		internal void RaisePlayEvent(SoundInstance instance)
		{
			if (OnPlay != null)
				OnPlay(instance);
		}

		internal void RaiseStopEvent(SoundInstance instance)
		{
			if (OnStop != null)
				OnStop(instance);
		}

		public event Action<SoundInstance> OnPlay;
		public event Action<SoundInstance> OnStop;

		//ncrunch: no coverage start
		public class SoundNotFoundOrAccessible : Exception
		{
			public SoundNotFoundOrAccessible(string soundName, Exception innerException)
				: base(soundName, innerException) { }
		}
	}
}