using System;

namespace DeltaEngine.Entities.Tests
{
	public class TestEntitySystem<EntityHandlerType> : EntitySystem
		where EntityHandlerType : EntityHandler, new()
	{
		public TestEntitySystem()
			: base(new TestHandlerResolver()) {}

		private class TestHandlerResolver : EntityHandlerResolver
		{
			public EntityHandler Resolve(Type handlerType)
			{
				if (handlerType == typeof(EntityHandlerType))
					return new EntityHandlerType();

				return null; //ncrunch: no coverage
			}
		}
	}

	public class TestEntitySystem<EntityHandlerType1, EntityHandlerType2> : EntitySystem
		where EntityHandlerType1 : EntityHandler, new()
		where EntityHandlerType2 : EntityHandler, new()
	{
		public TestEntitySystem()
			: base(new TestHandlerResolver()) {}

		private class TestHandlerResolver : EntityHandlerResolver
		{
			public EntityHandler Resolve(Type handlerType)
			{
				if (handlerType == typeof(EntityHandlerType1))
					return new EntityHandlerType1();

				if (handlerType == typeof(EntityHandlerType2))
					return new EntityHandlerType2();

				return null; //ncrunch: no coverage
			}
		}
	}
}