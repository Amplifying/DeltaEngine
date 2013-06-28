using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class ComponentTests
	{
		[Test]
		public void CreateEntityWithRotationComponent()
		{
			EntitySystem.Use(new TestEntitySystem<Rotate>());
			var entity = new EmptyEntity().Add(0.5f).Start<Rotate>();
			Assert.AreEqual(0.5f, entity.Get<float>());
			EntitySystem.Current.Run();
			Assert.AreEqual(0.6f, entity.Get<float>());
		}

		public class Rotate : Behavior<Entity>
		{
			internal override void Handle(Entity entity)
			{
				entity.Set(entity.Get<float>() + 0.1f);
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}

		[Test]
		public void CanCheckEntityHandlersPriority()
		{
			EntitySystem entitySystem = new TestEntitySystem<Rotate>();
			Assert.AreEqual(Priority.First, entitySystem.GetHandler<Rotate>().Priority);
		}
	}
}