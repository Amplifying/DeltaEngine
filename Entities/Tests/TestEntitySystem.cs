using System;

namespace DeltaEngine.Entities.Tests
{
	public class TestEntitySystem<HandlerType> : EntitySystem
		where HandlerType : Handler, new()
	{
		public TestEntitySystem()
			: base(new TestHandlerResolver()) {}

		private class TestHandlerResolver : HandlerResolver
		{
			public Handler Resolve(Type handlerType)
			{
				if (handlerType == typeof(HandlerType))
					return new HandlerType();

				return null; //ncrunch: no coverage
			}
		}
	}

	public class TestEntitySystem<HandlerType1, HandlerType2> : EntitySystem
		where HandlerType1 : Handler, new() where HandlerType2 : Handler, new()
	{
		public TestEntitySystem()
			: base(new TestHandlerResolver()) {}

		private class TestHandlerResolver : HandlerResolver
		{
			public Handler Resolve(Type handlerType)
			{
				if (handlerType == typeof(HandlerType1))
					return new HandlerType1();

				if (handlerType == typeof(HandlerType2))
					return new HandlerType2();

				return null; //ncrunch: no coverage
			}
		}
	}

	public class TestEntitySystem<HandlerType1, HandlerType2, HandlerType3> : EntitySystem
		where HandlerType1 : Handler, new() where HandlerType2 : Handler, new()
		where HandlerType3 : Handler, new()
	{
		public TestEntitySystem()
			: base(new TestHandlerResolver()) {}

		private class TestHandlerResolver : HandlerResolver
		{
			public Handler Resolve(Type handlerType)
			{
				if (handlerType == typeof(HandlerType1))
					return new HandlerType1();

				if (handlerType == typeof(HandlerType2))
					return new HandlerType2();

				if (handlerType == typeof(HandlerType3))
					return new HandlerType3();

				return null; //ncrunch: no coverage
			}
		}
	}
}