using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class EntityListenerTests
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

			private class TestHandlerResolver : EntityHandlerResolver
			{
				public EntityHandler Resolve(Type handlerType)
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
		private class SendMessage : EntityHandler
		{
			public override void Handle(List<Entity> activeEntities)
			{
				activeEntities[0].MessageAllListeners(new Message());
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		private class Message {}

		private class ListenForMessage : EntityListener
		{
			public override void ReceiveMessage(Entity entity, object message)
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
			return new EmptyEntity().Add(0).Add<SendMessage, ListenForMessage>();
		}

		[Test]
		public void AddingTheSameHandlersTwiceDoesNotAffect()
		{
			var entity = CreateEntityWithHandlers();
			entity.Add<SendMessage, ListenForMessage>();
			Assert.AreEqual(0, entity.Get<int>());
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void RemovingListenerStopsListenedBehaviourOccurring()
		{
			var entity = CreateEntityWithHandlers();
			EntitySystem.Current.Run();
			entity.Remove<ListenForMessage>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void GetAllEntitiesWithMultipleHandlersWithCertainTag()
		{
			var test2 = new EmptyEntity { Tag = "test" };
			test2.Add<SendMessage, ListenForMessage>();
			Assert.AreEqual(1, EntitySystem.Current.GetEntitiesWithTag("test").Count);
		}
	}
}