using System;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class AppTests
	{
		[Test]
		public void StartWithRunner()
		{
			TestAppOnce.Start<DummyRunner>();
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
			TestAppOnce.Start((DummyRunner dummy) => {}, () => {});
		}

		[Test]
		public void StartWithTwoClasses()
		{
			TestAppOnce.Start((DummyRunner dummy1, DummyPresenter dummy2) => { });
		}

		[Test]
		public void StartWithThreeClasses()
		{
			TestAppOnce.Start((DummyRunner dummy1, DummyRunner dummy2, DummyRunner dummy3) => {});
		}

		[Test]
		public void StartTwice()
		{
			TestAppOnce.Start<DummyRunner>();
			TestAppOnce.Start<DummyRunner>();
		}

		[Test]
		public void RegisterUnknownClassToMakeResolveSucceed()
		{
			TestAppOnce.Start((ClassWithInnerClass.UnknownInnerClass dummy) => {});
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
				resolver.Init<DummyRunner>();
				Assert.Throws<AutofacResolver.RegisterCallsMustBeBeforeInit>(
					resolver.RegisterAllUnknownTypesAutomatically);
				resolver.Resolve<ClassWithInnerClass.UnknownInnerClass>();
			}
		}

		[Test]
		public void GetRidOfDummyClassWarning()
		{
			Assert.IsNotNull(new DummyRunner());
			Assert.IsNotNull(new DummyPresenter());
			Assert.IsNotNull(new ClassWithInnerClass(new ClassWithInnerClass.UnknownInnerClass()));
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void CreateWindowAndDevice()
		{
			TestAppOnce.Start((Window window, Device device) => Assert.IsTrue(window.IsVisible));
		}
		
		[Test, Category("Slow")]
		public void ResizeWindow()
		{
			TestAppOnce.Start((Window window) => window.TotalSize = new Size(800, 600));
		}

		/// <summary>
		/// Only warns in Debug mode if Category("Slow") is NOT used! A single test might take 30-70ms
		/// (assembly loading), but if you run more tests it will take 11ms as specified.
		/// </summary>
		[Test, Category("Slow")]
		public void WarnIfTestRunsTooLong()
		{
			TestAppOnce.Start((DummyRunner dummy) => Thread.Sleep(11));
		}
	}
}