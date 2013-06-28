using System;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class EventListenerTests
	{
		[SetUp]
		public void CreateSystem()
		{
			EntitySystem.Use(new TestEntitySystem());
		}

		private class TestEntitySystem : EntitySystem
		{
			public TestEntitySystem()
				: base(new TestHandlerResolver()) {}

			private class TestHandlerResolver : HandlerResolver
			{
				public Handler Resolve(Type handlerType)
				{
					if (handlerType == typeof(SendMessage))
						return new SendMessage();

					if (handlerType == typeof(ListenForMessage))
						return new ListenForMessage();

					return null; //ncrunch: no coverage
				}
			}
		}

		/// <summary>
		/// Checks if any entity was clicked. Pretends activeEntities[0] was clicked and sends a
		/// ClickedMessage to all EntityListeners attached to that entity.
		/// </summary>
		private class SendMessage : Behavior<Entity>
		{
			internal override void Handle(Entity entity)
			{
				entity.MessageAllListeners(new Message());
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}

		private class Message {}

		private class ListenForMessage : EventListener
		{
			internal override void ReceiveMessage(Entity entity, object message)
			{
				if (message is Message)
					entity.Set(entity.Get<int>() + 1);
			}
		}

		[Test]
		public void OneHandlerMessagesAnotherHandler()
		{
			var entity = CreateEntityWithHandlers();
			Assert.AreEqual(0, entity.Get<int>());
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		private static Entity CreateEntityWithHandlers()
		{
			return new EmptyEntity().Add(0).Start<SendMessage, ListenForMessage>();
		}

		[Test]
		public void AddingTheSameHandlersTwiceDoesNotAffect()
		{
			var entity = CreateEntityWithHandlers();
			entity.Start<SendMessage, ListenForMessage>();
			Assert.AreEqual(0, entity.Get<int>());
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void RemovingListenerStopsListenedBehaviourOccurring()
		{
			var entity = CreateEntityWithHandlers();
			EntitySystem.Current.Run();
			entity.Stop<ListenForMessage>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}
	}
}