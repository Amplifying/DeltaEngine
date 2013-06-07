using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class ComponentTests
	{
		[Test]
		public void CreateEntityWithRotationComponent()
		{
			EntitySystem.Use(new TestEntitySystem<Rotate>());
			var entity = new EmptyEntity().Add(0.5f).Add<Rotate>();
			Assert.AreEqual(0.5f, entity.Get<float>());
			EntitySystem.Current.Run();
			Assert.AreEqual(0.6f, entity.Get<float>());
		}

		public class Rotate : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				entity.Set(entity.Get<float>() + 0.1f);
			}

			public override EntityHandlerPriority Priority
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