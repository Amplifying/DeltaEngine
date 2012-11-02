using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.WindowsDirectX.Tests
{
	public class SharpDXResolverTests
	{
		[Test]
		public void ResolveTime()
		{
			MakeSureTypeCanBeResolved<Time>();
		}

		private static void MakeSureTypeCanBeResolved<T>()
			where T : Runner
		{
			using (var resolver = new SharpDXResolver().Init<T>())
				Assert.IsNotNull(resolver.Resolve<T>());
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void ResolveWindow()
		{
			MakeSureTypeCanBeResolved<Window>();
		}

		[Test, Category("Slow")]
		public void ResolveDevice()
		{
			MakeSureTypeCanBeResolved<Device>();
		}

		[Test, Category("Slow")]
		public void ResolveRenderer()
		{
			MakeSureTypeCanBeResolved<Renderer>();
		}
	}
}