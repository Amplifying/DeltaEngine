using System;
using DeltaEngine.Core;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Holds the audio device and automatically disposes all finished playing sound instances.
	/// </summary>
	public abstract class SoundDevice : Runner, IDisposable
	{
		public abstract void Run();
		public abstract void Dispose();
	}
}