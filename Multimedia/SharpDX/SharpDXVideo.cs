using System;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

namespace DeltaEngine.Multimedia.SharpDX
{
	public class SharpDXVideo : Video
	{
		public SharpDXVideo(string filename, Drawing drawing, Renderer renderer)
			: base(filename, renderer)
		{
			this.drawing = drawing;
		}

		private readonly Drawing drawing;

		protected override VideoSurface PlayNativeVideo(float volume)
		{
			return new SharpDXVideoSurface(this, drawing, renderer);
		}

		protected override void StopNativeVideo()
		{
			throw new NotImplementedException();
		}

		public override bool IsPlaying()
		{
			throw new NotImplementedException();
		}

		protected override void Run()
		{
			throw new NotImplementedException();
		}

		public override float DurationInSeconds
		{
			get { throw new NotImplementedException(); }
		}

		public override float PositionInSeconds
		{
			get { throw new NotImplementedException(); }
		}

		public override void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
