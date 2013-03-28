using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Core.Tests
{
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
}