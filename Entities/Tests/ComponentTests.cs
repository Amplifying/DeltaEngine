using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class ComponentTests
	{
		[Test]
		public void CreateEntityWithRotationComponent()
		{
			EntitySystem entitySystem = new TestEntitySystem<Rotate>();
			var entity = new EntityTests.EmptyEntity().Add(0.5f).Add<Rotate>();
			entitySystem.Add(entity);
			Assert.AreEqual(0.5f, entity.Get<float>());
			entitySystem.Run();
			Assert.AreEqual(0.6f, entity.Get<float>());
		}

		public class Rotate : EntityHandler
		{
			public void Handle(List<Entity> entities)
			{
				foreach (var entity in entities)
					entity.Set(entity.Get<float>() + 0.1f);
			}

			public EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		[Test]
		public void CanCheckEntityHandlersPriority()
		{
			EntitySystem entitySystem = new TestEntitySystem<Rotate>();
			Assert.AreEqual(EntityHandlerPriority.First, entitySystem.GetHandler<Rotate>().Priority);
		}
	}
}