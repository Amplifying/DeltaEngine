using System;
using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class ResolverTests : TestStarter
	{
		[IntegrationTest]
		public void ResolveTime(Type resolver)
		{
			MakeSureTypeCanBeResolved<Time>(resolver);
		}

		private void MakeSureTypeCanBeResolved<T>(Type resolverType)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return;

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.Start<T>();
				Assert.IsNotNull(resolver.Resolve<T>());
			}
		}

		private static AutofacStarter CreateResolver(Type resolverType)
		{
			if (resolverType == typeof(TestResolver))
				return (TestResolver)Activator.CreateInstance(resolverType);

			return (AutofacStarter)Activator.CreateInstance(resolverType); // ncrunch: no coverage
		}

		[IntegrationTest]
		public void ResolveWindow(Type resolver)
		{
			MakeSureTypeCanBeResolved<Window>(resolver);
		}

		[IntegrationTest]
		public void ResolveDevice(Type resolver)
		{
			MakeSureTypeCanBeResolved<Device>(resolver);
		}

		[IntegrationTest]
		public void ResolveRenderer(Type resolver)
		{
			MakeSureTypeCanBeResolved<Renderer>(resolver);
		}

		[IntegrationTest]
		public void CloseApp(Type resolverType)
		{
			Start(resolverType, (AutofacStarter resolver) => resolver.Close());
		}

		[IntegrationTest]
		public void RegisterAfterResolveIsNotAllowed(Type resolverType)
		{
			Start(resolverType, (AutofacResolver resolver) =>
			{
				resolver.Resolve<Device>();
				Assert.Throws<AutofacResolver.UnableToRegisterMoreTypesAppAlreadyStarted>(
					resolver.Register<object>);
				Assert.Throws<AutofacResolver.UnableToRegisterMoreTypesAppAlreadyStarted>(
					resolver.RegisterSingleton<object>);
			});
		}
	}
}