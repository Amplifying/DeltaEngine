using System;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework.Content;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native implementation of an audio context, nothing to do on xna here.
	/// </summary>
	public class XnaSoundDevice : SoundDevice
	{
		public XnaSoundDevice(Window window)
		{
			serviceProvider = new XnaServiceProvider();
			Content = new ContentManager(serviceProvider, "Content");
		}

		private class XnaServiceProvider : IServiceProvider
		{
			public object GetService(Type serviceType)
			{
				return null;
			}
		}

		private readonly XnaServiceProvider serviceProvider;

		public ContentManager Content { get; private set; }

		public override void Run() {}

		public override void Dispose()
		{
			if (Content != null)
				Content.Dispose();
			Content = null;
		}
	}
}