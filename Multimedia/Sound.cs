using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to load and play simple sound effects by creating a SoundInstance per play.
	/// </summary>
	public abstract class Sound : ContentData
	{
		protected Sound(string filename, SoundDevice device)
			: base(filename)
		{
			this.device = device;
		}

		protected readonly SoundDevice device;
		public abstract float LengthInSeconds { get; }
		public int NumberOfPlayingInstances
		{
			get { return internalInstances.Count + externalInstances.Count; }
		}

		public override void Dispose()
		{
			foreach (var instance in new List<SoundInstance>(internalInstances))
				instance.Dispose();
			foreach (var instance in new List<SoundInstance>(externalInstances))
				instance.Dispose();
		}

		public void Play(float volume = 1.0f, float panning = 0.0f)
		{
			SoundInstance freeInstance = GetInternalNonPlayingInstance();
			freeInstance.Volume = volume;
			freeInstance.Panning = panning;
			PlayInstance(freeInstance);
		}

		private SoundInstance GetInternalNonPlayingInstance()
		{
			SoundInstance freeInstance = internalInstances.FirstOrDefault(i => !IsPlaying(i));
			if (freeInstance != null)
				return freeInstance;
			createInternalInstance = true;
			return new SoundInstance(this);
		}

		private bool createInternalInstance;

		public abstract void PlayInstance(SoundInstance instanceToPlay);

		public void StopAll()
		{
			foreach (var instance in internalInstances)
				instance.Stop();
			foreach (var instance in externalInstances)
				instance.Stop();
		}

		public abstract void StopInstance(SoundInstance instanceToStop);

		internal void Add(SoundInstance instanceToAdd)
		{
			if (createInternalInstance)
				internalInstances.Add(instanceToAdd);
			else
				externalInstances.Add(instanceToAdd);
			createInternalInstance = false;
			CreateChannel(instanceToAdd);
		}

		protected abstract void CreateChannel(SoundInstance instanceToFill);

		private readonly List<SoundInstance> internalInstances = new List<SoundInstance>();

		private readonly List<SoundInstance> externalInstances = new List<SoundInstance>();

		internal void Remove(SoundInstance instanceToRemove)
		{
			internalInstances.Remove(instanceToRemove);
			externalInstances.Remove(instanceToRemove);
			RemoveChannel(instanceToRemove);
		}

		protected abstract void RemoveChannel(SoundInstance instanceToRemove);

		public abstract bool IsPlaying(SoundInstance instance);

		public bool IsAnyInstancePlaying
		{
			get { return internalInstances.Any(IsPlaying) || externalInstances.Any(IsPlaying); }
		}
	}
}