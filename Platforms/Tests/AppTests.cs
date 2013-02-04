using System;
using System.Threading;
using DeltaEngine.Core;
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

		private class DummyRunner : Runner, IDisposable
		{
			public void Run() {}
			public void Dispose() {}
		}

		private class DummyPresenter : Presenter
		{
			public void Run() {}
			public void Present() {}
		}
		
		[Test]
		public void StartWithOneClass()
		{
			Start(typeof(TestResolver), (DummyRunner dummy) => {}, () => {});
		}
		
		[Test]
		public void StartWithTwoClasses()
		{
			Start(typeof(TestResolver), (DummyRunner dummy1, DummyPresenter dummy2) => { });
		}

		[Test]
		public void StartWithThreeClasses()
		{
			Start(typeof(TestResolver),
				(DummyRunner dummy1, DummyRunner dummy2, DummyRunner dummy3) => { });
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
			Start(typeof(TestResolver), (ClassWithInnerClass.UnknownInnerClass dummy) => {});
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

			public void Run() { }
		}

		[Test]
		public void GetRidOfDummyClassWarning()
		{
			Assert.IsNotNull(new DummyRunner());
			Assert.IsNotNull(new DummyPresenter());
			Assert.IsNotNull(new ClassWithInnerClass(new ClassWithInnerClass.UnknownInnerClass()));
			Assert.IsNotNull(new CustomRunner("test"));
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