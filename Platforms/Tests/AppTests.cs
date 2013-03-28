using System;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Logging;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class AppTests : TestStarter
	{
		[IntegrationTest]
		public void StartWithRunner(Type resolver)
		{
			Start<DummyRunner>(resolver);
		}

		[IntegrationTest]
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

		[IntegrationTest]
		public void StartWithOneClass(Type resolver)
		{
			Start(resolver, (First first) => { }, () => { });
		}

		[IntegrationTest]
		public void StartWithTwoClasses(Type resolver)
		{
			Start(resolver, (First first, Second second) => { });
		}

		[IntegrationTest]
		public void StartWithOneAndOneClasses(Type resolver)
		{
			Start(resolver, (First first) => { }, (Second second) => { });
		}

		[IntegrationTest]
		public void StartWithThreeClasses(Type resolver)
		{
			Start(resolver, (First first, Second second, Third third) => { });
		}

		[IntegrationTest]
		public void StartWithTwoAndOneClasses(Type resolver)
		{
			Start(resolver, (First first, Second second) => { }, (Third third) => { });
		}

		[IntegrationTest]
		public void StartWithOneAndTwoClasses(Type resolver)
		{
			Start(resolver, (First first) => { }, (Second second, Third third) => { });
		}

		[IntegrationTest]
		public void StartWithThreeAndOneClasses(Type resolver)
		{
			Start(resolver, (First first, Second second, Third third) => { },
				(Forth forth) => { });
		}

		[IntegrationTest]
		public void StartWithTwoAndTwoClasses(Type resolver)
		{
			Start(resolver, (First first, Second second) => { },
				(Third third, Forth forth) => { });
		}

		[IntegrationTest]
		public void StartWithOneAndThreeClasses(Type resolver)
		{
			Start(resolver, (First first) => { },
				(Second second, Third third, Forth forth) => { });
		}

		[IntegrationTest]
		public void StartWithThreeAndTwoClasses(Type resolver)
		{
			Start(resolver, (First first, Second second, Third third) => { },
				(Forth forth, Fifth fifth) => { });
		}

		[IntegrationTest]
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
			Start<DummyRunner>(typeof(TestResolver));
			Start<DummyRunner>(typeof(TestResolver));
		}

		[Test]
		public void RegisterTwice()
		{
			using (var resolver = new TestResolver())
			{
				resolver.Start<DummyRunner>();
				resolver.Start<DummyRunner>();
			}
		}

		[Test]
		public void RegisterUnknownClassToMakeResolveSucceed()
		{
			Start(typeof(TestResolver), (ClassWithInnerClass.UnknownInnerClass dummy) => { });
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
			using (var resolver = new TestResolver())
			{
				resolver.RegisterAllUnknownTypesAutomatically();
				resolver.Start<DummyRunner>();
				Assert.Throws<AutofacResolver.RegisterCallsMustBeBeforeInit>(
					resolver.RegisterAllUnknownTypesAutomatically);
				resolver.Resolve<ClassWithInnerClass.UnknownInnerClass>();
			}
		}

		[Test]
		public void ResolveWithCustomParameter()
		{
			var resolver = new TestResolver();
			resolver.RegisterAllUnknownTypesAutomatically();
			var runner = resolver.Resolve<CustomRunner>("test");
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
			var resolver = new TestResolver();
			var myClass = new ClassWithInnerClass.UnknownInnerClass();
			resolver.RegisterMock(myClass);
			Assert.Throws<AssertionException>(() => resolver.RegisterMock(myClass));
		}

		[IntegrationTest]
		public void ExceptionInRunCodeShouldNotCrashApp(Type resolver)
		{
			if (resolver == typeof(TestResolver))
			{
				Assert.Throws<Exception>(() =>
					Start(resolver, (Logger logger) => {}, () => { throw new Exception("Should not crash"); }));
			}
		}

		[TestCase(typeof(OpenTKResolver), Category = "Visual")]
		public void TestVisualCategoryInTestStarter(Type resolver)
		{
			Start(resolver, (Window w) => { });
		}

		//ncrunch: no coverage start
		/// <summary>
		/// Only warns in Debug mode if Category("Slow") is NOT used! A single test might take 30-70ms
		/// (assembly loading), but if you run more tests it will take 11ms as specified.
		/// </summary>
		[Test, Category("Slow")]
		public void WarnIfTestRunsTooLong()
		{
			Start(typeof(TestResolver), (DummyRunner dummy) => Thread.Sleep(11));
		}
	}
}