using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics.SharpDX;
using DeltaEngine.Logging;
using DeltaEngine.Multimedia.AviVideo;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System.Diagnostics;

namespace DeltaEngine.Multimedia.SharpDX
{
	public class SharpDXVideo : Video
	{
		public SharpDXVideo(string filename, SoundDevice device, Logger log, SharpDXDrawing drawing,
			SharpDXDevice graphicsDevice, EntitySystem entitySystem, ScreenSpace screen)
			: base(filename, device)
		{
			this.entitySystem = entitySystem;
			this.screen = screen;
			image = new VideoImage(drawing, graphicsDevice, log);
			CreateBuffers();
		}

		private readonly EntitySystem entitySystem;
		private readonly ScreenSpace screen;
		private readonly VideoImage image;
		private SourceVoice source;

		private void CreateBuffers()
		{
			buffers = new AudioBuffer[NumberOfBuffers];
			for (int i = 0; i < NumberOfBuffers; i++)
				buffers[i] = new AudioBuffer();
		}

		private AudioBuffer[] buffers;
		private const int NumberOfBuffers = 2;

		protected override void LoadData(Stream fileData)
		{
			try
			{
				var aviManager = new AviFile("Content/DefaultVideo.avi");
				video = aviManager.GetVideoStream();
				audio = aviManager.GetAudioStream();
				source = new SourceVoice((device as XAudioDevice).XAudio2,
					new WaveFormat(audio.SamplesPerSecond, 16, audio.Channels), false);
			}
			catch (Exception ex)
			{
				if (!Debugger.IsAttached)
					return;
				else
					throw new VideoNotFoundOrAccessible(Name, ex);
			}
		}

		private VideoStream video;
		private AudioStream audio;

		protected override void DisposeData()
		{
			base.DisposeData();
			video = null;
			audio = null;
			if (source != null)
				DisposeSource();
		}

		private void DisposeSource()
		{
			for (int i = 0; i < NumberOfBuffers; i++)
				buffers[i] = null;

			source.FlushSourceBuffers();
			source.DestroyVoice();
			source.Dispose();
			source = null;
		}

		protected override void PlayNativeVideo(float volume)
		{
			if (isPlaying)
				return;

			source.Start();
			isPlaying = true;
			video.GetFrameOpen();
			elapsedSeconds = 0f;
			surface = new Sprite(image, screen.Viewport, Color.White);
			entitySystem.Add(surface);
		}

		private bool isPlaying;
		private Sprite surface;
		private float elapsedSeconds;

		protected override void StopNativeVideo()
		{
			if (!isPlaying)
				return;

			isPlaying = false;
			source.Stop();
			source.FlushSourceBuffers();
			if (surface != null)
				entitySystem.Remove(surface);
			surface = null;
			video.GetFrameClose();
		}

		public override bool IsPlaying()
		{
			return isPlaying;
		}

		protected override void Run()
		{
			if (isPlaying)
				RunIfPlaying();
		}

		private void RunIfPlaying()
		{
			while (source.State.BuffersQueued < NumberOfBuffers)
			{
				PutInStreamIfDataAvailable();
				if (isAbleToStream)
					continue;
				Stop();
				break;
			}

			elapsedSeconds += Time.Current.Delta;
			UpdateVideoTexture();
		}

		private unsafe void PutInStreamIfDataAvailable()
		{
			AudioBuffer currentBuffer = buffers[nextBufferIndex];
			try
			{
				byte[] bufferData = audio.GetStreamData();
				fixed (byte* ptr = &bufferData[0])
					currentBuffer.AudioDataPointer = (IntPtr)ptr;

				currentBuffer.AudioBytes = bufferData.Length;
				int blockAlign = audio.Channels * 2;
				currentBuffer.PlayLength = bufferData.Length / blockAlign;
			}
			catch
			{
				isAbleToStream = false;
				return;
			}

			isAbleToStream = true;
			source.SubmitSourceBuffer(currentBuffer, null);
			nextBufferIndex = (nextBufferIndex + 1) % NumberOfBuffers;
		}

		private int nextBufferIndex;
		private bool isAbleToStream;

		private void UpdateVideoTexture()
		{
			var frameIndex = (int)(PositionInSeconds * video.FrameRate);
			if (lastFrameIndex >= frameIndex)
				return;

			lastFrameIndex = frameIndex;
			image.UpdateTexture(video, frameIndex);
		}

		private int lastFrameIndex = -1;

		public override float DurationInSeconds
		{
			get { return (float)(video.CountFrames / video.FrameRate); }
		}

		public override float PositionInSeconds
		{
			get { return MathExtensions.Round(elapsedSeconds.Clamp(0f, DurationInSeconds), 2); }
		}
	}
}