using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test sound playback. Xna sound loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	public class SoundTests : TestStarter
	{
		[VisualTest]
		public void PlaySoundAndDispose(Type resolver)
		{
			Start(resolver, (Content content) =>
			{ 
				var sound = content.Load<Sound>("DefaultSound");
				sound.Dispose();
			});
		}

		[VisualTest]
		public void PlaySoundLeft(Type resolver)
		{
			Start(resolver, (Content content) => content.Load<Sound>("DefaultSound").Play(1, -1));
		}

		[VisualTest]
		public void PlaySoundRightAndPitched(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance = new SoundInstance(sound) { Panning = 1.0f, Pitch = 2.0f };
				instance.Play();
			});
		}

		[VisualTest]
		public void PlaySoundInstance(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance = new SoundInstance(sound);
				Assert.AreEqual(false, instance.IsPlaying);
				instance.Play();
				Assert.AreEqual(true, instance.IsPlaying);
			});
		}

		[VisualTest]
		public void PlayMultipleSoundInstances(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance1 = new SoundInstance(sound);
				var instance2 = new SoundInstance(sound);
				Assert.AreEqual(false, instance1.IsPlaying);
				instance1.Play();
				Assert.AreEqual(true, instance1.IsPlaying);
				Assert.AreEqual(false, instance2.IsPlaying);
				instance2.Volume = 0.5f;
				instance2.Panning = -1.0f;
				instance2.Play();
				Assert.AreEqual(true, instance2.IsPlaying);
			});
		}

		[VisualTest]
		public void NumberOfPlayingInstances(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				Assert.AreEqual(0, sound.NumberOfPlayingInstances);
				sound.Play();
				Assert.AreEqual(1, sound.NumberOfPlayingInstances);
				sound.Play();
				Assert.AreEqual(2, sound.NumberOfPlayingInstances);
			});
		}

		[VisualTest]
		public void PlayAndStop(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				Assert.IsFalse(sound.IsAnyInstancePlaying);
				sound.Play();
				Assert.IsTrue(sound.IsAnyInstancePlaying);
				sound.StopAll();
				Assert.IsFalse(sound.IsAnyInstancePlaying);
				sound.Play();
			});
		}

		[VisualTest]
		public void PlayAndStopInstance(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance = new SoundInstance(sound);
				Assert.IsFalse(sound.IsAnyInstancePlaying);
				instance.Play();
				Assert.IsTrue(sound.IsAnyInstancePlaying);
				sound.StopAll();
				Assert.IsFalse(sound.IsAnyInstancePlaying);
			});
		}

		[VisualTest]
		public void DisposeSoundInstance(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				var instance = new SoundInstance(sound);
				Assert.AreEqual(1, sound.NumberOfPlayingInstances);
				instance.Dispose();
				Assert.AreEqual(0, sound.NumberOfPlayingInstances);
			});
		}
		
		[VisualTest]
		public void DisposeSoundInstancesFromSoundClass(Type resolver)
		{
			Start(resolver, (Content content) =>
			{
				var sound = content.Load<Sound>("DefaultSound");
				new SoundInstance(sound);
				sound.Play();
				Assert.AreEqual(2, sound.NumberOfPlayingInstances);
				sound.Dispose();
				Assert.AreEqual(0, sound.NumberOfPlayingInstances);
			});
		}

	}
}