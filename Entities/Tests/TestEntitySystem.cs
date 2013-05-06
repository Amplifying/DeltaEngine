using System;

namespace DeltaEngine.Entities.Tests
{
	public class TestEntitySystem<EntityHandlerType> : EntitySystem
		where EntityHandlerType : EntityHandler, new()
	{
		public TestEntitySystem()
			: base(new TestHandlerResolver()) { }

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
}