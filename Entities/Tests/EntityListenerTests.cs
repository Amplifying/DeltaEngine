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
			entitySystem = new TestEntitySystem();
		}

		private EntitySystem entitySystem;

		private class TestEntitySystem : EntitySystem
		{
			public TestEntitySystem()
				: base(new TestHandlerResolver()) {}

			private class TestHandlerResolver : EntityHandlerResolver
			{
				public EntityHandler Resolve(Type handlerType)
				{
					if (handlerType == typeof(ListenForMouseClick))
						return new ListenForMouseClick();

					if (handlerType == typeof(SpeedUpSpinRateOnMouseClick))
						return new SpeedUpSpinRateOnMouseClick();

					return null; //ncrunch: no coverage
				}
			}
		}

		/// <summary>
		/// Checks if any entity was clicked. Pretends activeEntities[0] was clicked and sends a
		/// ClickedMessage to all EntityListeners attached to that entity.
		/// </summary>
		private class ListenForMouseClick : EntityHandler
		{
			public void Handle(List<Entity> activeEntities)
			{
				activeEntities[0].MessageAllListeners(new ClickedMessage());
			}

			public EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		private class ClickedMessage {}

		/// <summary>
		/// Speeds up the spin rate of any entity clicked. Doesn't do anything unless sent a message.
		/// </summary>
		private class SpeedUpSpinRateOnMouseClick : EntityListener
		{
			public void Handle(List<Entity> activeEntities) {}

			public void ReceiveMessage(Entity entity, object message)
			{
				if (message is ClickedMessage)
					entity.Set(entity.Get<float>() + 1);
			}

			public EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		[Test]
		public void OneHandlerMessagesAnotherHandler()
		{
			var entity = new EntityTests.EmptyEntity();
			AddHandlersToEntity(entity);
			Assert.AreEqual(0, entity.Get<float>());
			entitySystem.Run();
			Assert.AreEqual(1, entity.Get<float>());
		}

		private void AddHandlersToEntity(Entity entity)
		{
			entity.Add(0.0f).Add<ListenForMouseClick, SpeedUpSpinRateOnMouseClick>();
			entitySystem.Add(entity);
		}

		[Test]
		public void AddingTheSameHandlersTwiceDoesNotAffect()
		{
			var entity = new EntityTests.EmptyEntity();
			AddHandlersToEntity(entity);
			AddHandlersToEntity(entity);
			Assert.AreEqual(0, entity.Get<float>());
			entitySystem.Run();
			Assert.AreEqual(1, entity.Get<float>());
		}

		[Test]
		public void RemovingListenerStopsListenedBehaviourOccurring()
		{
			var entity = new EntityTests.EmptyEntity();
			AddHandlersToEntity(entity);
			entitySystem.Run();
			entity.Remove<SpeedUpSpinRateOnMouseClick>();
			entitySystem.Run();
			Assert.AreEqual(1, entity.Get<float>());
		}

		[Test]
		public void GetAllEntitiesWithMultipleHandlersWithCertainTag()
		{
			var test2 = new EntityTests.EmptyEntity { Tag = "test" };
			test2.Add<ListenForMouseClick, SpeedUpSpinRateOnMouseClick>();
			entitySystem.Add(test2);
			Assert.AreEqual(1, entitySystem.GetEntitiesWithTag("test").Count);
		}
	}
}