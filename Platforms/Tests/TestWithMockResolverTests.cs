using System;
using System.Threading;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Graphics;
using DeltaEngine.Logging;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class TestWithMockResolverTests : TestWithMockResolver
	{
		[TestCase(typeof(MockResolver))]
		public void StartWithRunner(Type resolver)
		{
			Start<DummyRunner>(resolver);
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithTwoRunners(Type resolver)
		{
			Start<DummyRunner>(resolver, 2);
		}

		private class DummyRunner : Runner, IDisposable
		{
			public void Run() {}
			public void Dispose() {}
		}

		private class DummyPresenter : Presenter
		{
			public DummyPresenter()
			{
				Run();
				Present();
			}

			public void Run() {}
			public void Present() {}
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithOneClass(Type resolver)
		{
			Start(resolver, (First first) => { }, () => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithTwoClasses(Type resolver)
		{
			Start(resolver, (First first, Second second) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithOneAndOneClasses(Type resolver)
		{
			Start(resolver, (First first) => { }, (Second second) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithThreeClasses(Type resolver)
		{
			Start(resolver, (First first, Second second, Third third) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithFourClasses(Type resolver)
		{
			Start(resolver, (First first, Second second, Third third, Forth forth) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithFiveClasses(Type resolver)
		{
			Start(resolver, (First first, Second second, Third third, Forth forth, Fifth fifth) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithTwoAndOneClasses(Type resolver)
		{
			Start(resolver, (First first, Second second) => { }, (Third third) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithOneAndTwoClasses(Type resolver)
		{
			Start(resolver, (First first) => { }, (Second second, Third third) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithThreeAndOneClasses(Type resolver)
		{
			Start(resolver, (First first, Second second, Third third) => { }, (Forth forth) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithTwoAndTwoClasses(Type resolver)
		{
			Start(resolver, (First first, Second second) => { }, (Third third, Forth forth) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithOneAndThreeClasses(Type resolver)
		{
			Start(resolver, (First first) => { }, (Second second, Third third, Forth forth) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithThreeAndTwoClasses(Type resolver)
		{
			Start(resolver, (First first, Second second, Third third) => { },
				(Forth forth, Fifth fifth) => { });
		}

		[TestCase(typeof(MockResolver))]
		public void StartWithTwoAndThreeClasses(Type resolver)
		{
			Start(resolver, (First first, Second second) => { },
				(Third third, Forth forth, Fifth fifth) => { });
		}

		private class First {}

		private class Second {}

		private class Third {}

		private class Forth {}

		private class Fifth {}

		[Test]
		public void CreateUnusedClasses()
		{
			Assert.IsNotNull(new First());
			Assert.IsNotNull(new Second());
			Assert.IsNotNull(new Third());
			Assert.IsNotNull(new Forth());
			Assert.IsNotNull(new Fifth());
		}

		[Test]
		public void StartTwice()
		{
			Start<DummyRunner>(typeof(MockResolver));
			Start<DummyRunner>(typeof(MockResolver));
		}

		[Test]
		public void RegisterTwice()
		{
			using (var resolver = new MockResolver())
			{
				resolver.resolver.Start<DummyRunner>();
				resolver.resolver.Start<DummyRunner>();
			}
		}

		[Test]
		public void RegisterUnknownClassToMakeResolveSucceed()
		{
			Start(typeof(MockResolver), (ClassWithInnerClass.UnknownInnerClass dummy) => { });
		}

		private class ClassWithInnerClass
		{
			internal class UnknownInnerClass {}

			internal ClassWithInnerClass(UnknownInnerClass inner)
			{
				Assert.IsNotNull(inner);
			}
		}

		[Test]
		public void RegisterAllUnknownClasses()
		{
			using (var resolver = new MockResolver())
			{
				resolver.resolver.RegisterAllUnknownTypesAutomatically();
				resolver.resolver.Start<DummyRunner>();
				Assert.Throws<AutofacResolver.RegisterCallsMustBeBeforeInit>(
					resolver.resolver.RegisterAllUnknownTypesAutomatically);
				resolver.resolver.Resolve<ClassWithInnerClass.UnknownInnerClass>();
			}
		}

		[Test]
		public void ResolveWithCustomParameter()
		{
			var resolver = new MockResolver();
			resolver.resolver.RegisterAllUnknownTypesAutomatically();
			var runner = resolver.resolver.Resolve(typeof(CustomRunner), "test") as CustomRunner;
			runner.Run();
		}

		private class CustomRunner : Runner
		{
			public CustomRunner(string name)
			{
				Assert.IsNotNullOrEmpty(name);
			}

			public void Run() {}
		}

		[Test]
		public void GetRidOfDummyClassWarning()
		{
			Assert.IsNotNull(new DummyRunner());
			Assert.IsNotNull(new DummyPresenter());
			Assert.IsNotNull(new ClassWithInnerClass(new ClassWithInnerClass.UnknownInnerClass()));
			Assert.IsNotNull(new CustomRunner("test"));
		}

		[Test]
		public void RegisteringTwoEqualMockInstancesShouldFail()
		{
			var resolver = new MockResolver();
			var myClass = new ClassWithInnerClass.UnknownInnerClass();
			resolver.resolver.RegisterMock(myClass);
			Assert.Throws<AssertionException>(() => resolver.resolver.RegisterMock(myClass));
		}

		[Test]
		public void LoadMockXmlContent()
		{
			var resolver = new MockResolver();
			var content = resolver.resolver.Resolve<ContentLoader>();
			var testXml = content.Load<XmlContent>("Test");
			Assert.AreEqual("Root", testXml.Data.Name);
			Assert.AreEqual(1, testXml.Data.Children.Count);
			Assert.AreEqual("Hi", testXml.Data.Children[0].Name);
		}

		[TestCase(typeof(MockResolver))]
		public void ResolveTime(Type resolver)
		{
			MakeSureTypeCanBeResolved<Time>(resolver);
		}

		private void MakeSureTypeCanBeResolved<T>(Type resolverType)
		{
			if (IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(resolverType))
				return; //ncrunch: no coverage

			using (var resolver = CreateResolver(resolverType))
			{
				resolver.Start<T>();
				Assert.IsNotNull(resolver.Resolve<T>());
			}
		}

		private static AutofacStarter CreateResolver(Type resolverType)
		{
			if (resolverType == typeof(MockResolver))
				return ((MockResolver)Activator.CreateInstance(resolverType)).resolver;

			return (AutofacStarter)Activator.CreateInstance(resolverType); // ncrunch: no coverage
		}

		[TestCase(typeof(MockResolver))]
		public void ResolveWindow(Type resolver)
		{
			MakeSureTypeCanBeResolved<Window>(resolver);
		}

		[TestCase(typeof(MockResolver))]
		public void ResolveDevice(Type resolver)
		{
			MakeSureTypeCanBeResolved<Device>(resolver);
		}

		[TestCase(typeof(MockResolver))]
		public void CloseApp(Type resolverType)
		{
			Start(resolverType, (Window window) => window.Dispose());
		}

		[Test]
		public void RegisterAfterResolveIsNotAllowed()
		{
			Start(typeof(MockResolver), (Device device) =>
			{
				Assert.AreEqual(device, mockResolver.resolver.Resolve<Device>());
				Assert.Throws<AutofacResolver.UnableToRegisterMoreTypesAppAlreadyStarted>(
					mockResolver.resolver.Register<object>);
				Assert.Throws<AutofacResolver.UnableToRegisterMoreTypesAppAlreadyStarted>(
					mockResolver.resolver.RegisterSingleton<object>);
			});
		}

		[Test]
		public void ResolveCompleteClass()
		{
			var resolver = new MockResolver();
			resolver.resolver.Resolve<TestWithMockResolverTests>();
		}

		[TestCase(typeof(MockResolver), Category = "Visual")]
		public void TestVisualCategoryInTestStarter(Type resolver)
		{
			Start(resolver, (Window w) => { });
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void ExceptionInRunCodeShouldNotCrashAppWhenNoDebuggerIsAttached()
		{
			Start(typeof(MockResolver), (Logger logger) => { },
				() => { throw new Exception("Should not crash"); });
			foreach (var line in mockResolver.log.lines)
				Console.WriteLine(line);
		}

		/// <summary>
		/// Only warns in Debug mode if Category("Slow") is NOT used! A single test might take 30-70ms
		/// (assembly loading), but if you run more tests it will take 11ms as specified.
		/// </summary>
		[Test, Category("Slow")]
		public void WarnIfTestRunsTooLong()
		{
			Start(typeof(MockResolver), (DummyRunner dummy) => Thread.Sleep(11));
		}
	}
}