using System;
using System.IO;
using DeltaEngine.Core;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native Xna implementation for sound effect playback.
	/// </summary>
	public class XnaSound : Sound
	{
		public XnaSound(string contentName, XnaSoundDevice device, ContentManager contentManager)
			: base(contentName, device)
		{
			this.contentManager = contentManager;
		}

		private readonly ContentManager contentManager;

		private SoundEffect effect;

		protected override bool CanLoadDataFromStream
		{
			get { return false; }
		}

		protected override void LoadData(Stream fileData)
		{
			throw new XnaVideo.XnaOnlyAllowsLoadingThroughContentNames();	
		}

		protected override void LoadFromContentName(string contentName)
		{
			try
			{
				effect = contentManager.Load<SoundEffect>(contentName);
			}
			catch (Exception ex)
			{
				//logger.Error(ex);
				if (Debugger.IsAttached)
					throw new XnaSoundContentNotFound(contentName, ex);
			}
		}

		public class XnaSoundContentNotFound : Exception
		{
			public XnaSoundContentNotFound(string contentName, Exception exception)
				: base(contentName, exception) {}
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			if (effect != null)
				effect.Dispose();
			effect = null;
		}

		public override float LengthInSeconds
		{
			get { return (float)effect.Duration.TotalSeconds; }
		}

		public override void PlayInstance(SoundInstance instanceToPlay)
		{
			var effectInstance = instanceToPlay.Handle as SoundEffectInstance;
			if (effectInstance == null)
				return;

			effectInstance.Volume = instanceToPlay.Volume.Clamp(0.0f, 1.0f);
			effectInstance.Pan = instanceToPlay.Panning.Clamp(-1.0f, +1.0f);
			effectInstance.Pitch = (instanceToPlay.Pitch - 1).Clamp(-1.0f, +1.0f);
			effectInstance.Play();
		}

		public override void StopInstance(SoundInstance instanceToStop)
		{
			var effectInstance = instanceToStop.Handle as SoundEffectInstance;
			if (effectInstance != null)
				effectInstance.Stop();
		}

		protected override void CreateChannel(SoundInstance instanceToFill)
		{
			instanceToFill.Handle = effect.CreateInstance();
		}

		protected override void RemoveChannel(SoundInstance instanceToRemove)
		{
			var effectInstance = instanceToRemove.Handle as SoundEffectInstance;
			if (effectInstance != null)
				effectInstance.Dispose();
			instanceToRemove.Handle = null;
		}

		public override bool IsPlaying(SoundInstance instance)
		{
			var effectInstance = instance.Handle as SoundEffectInstance;
			return effectInstance != null && effectInstance.State == SoundState.Playing;
		}
	}
}