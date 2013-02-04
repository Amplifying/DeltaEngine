using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class ContentLoaderTests
	{
		[Test]
		public void LoadImageContent()
		{
			var resolver = new FakeResolver();
			var image = new Content(resolver).Load<FakeImage>("Test");
			Assert.IsTrue(image.IsLoaded);
			image.Dispose();
			Assert.IsFalse(image.IsLoaded);
			Assert.IsNull(resolver.Resolve<ContentLoaderTests>());
		}

		[Test]
		public void LoadWithoutContentNameIsNotAllowed()
		{
			Assert.Throws<ContentData.ContentNameMissing>(() => new FakeImage(""));
		}
	}

	public class FakeResolver : Resolver
	{
		public FakeResolver()
		{
			registeredInstances.Add(new FakeImage("Test"));
		}

		private readonly List<object> registeredInstances = new List<object>();

		protected override object Resolve(Type baseType)
		{
			return null;
		}

		public override BaseType Resolve<BaseType>(object customParameter = null)
		{
			foreach (BaseType instance in registeredInstances.OfType<BaseType>())
				return instance;

			return (BaseType)Resolve(typeof(BaseType));
		}
	}

	public class FakeImage : ContentData
	{
		public FakeImage(string filename)
			: base(filename)
		{
			IsLoaded = true;
		}

		public bool IsLoaded { get; private set; }

		public override void Dispose()
		{
			IsLoaded = false;
		}
	}
}