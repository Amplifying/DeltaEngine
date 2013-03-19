using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native implementation of a SoundDevice using xna that calls the FrameworkDispatcher.
	/// </summary>
	public class XnaSoundDevice : SoundDevice
	{
		public XnaSoundDevice()
		{
			serviceProvider = new XnaServiceProvider();
			string contentPath = Path.Combine(Directory.GetCurrentDirectory(), "Content");
			Content = new ContentManager(serviceProvider, contentPath);
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