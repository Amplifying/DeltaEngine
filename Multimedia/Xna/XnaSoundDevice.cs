using System;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native implementation of a SoundDevice using xna that calls the FrameworkDispatcher.
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

		public override void Run()
		{
			FrameworkDispatcher.Update();
			base.Run();
		}

		public override void Dispose()
		{
			if (Content != null)
				Content.Dispose();
			Content = null;
		}
	}
}