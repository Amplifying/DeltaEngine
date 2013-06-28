using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class TriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateTrigger()
		{
			var trigger = new Trigger(EntityContainsColor);
			Assert.AreEqual(EntityContainsColor, trigger.Condition);
		}

		private static readonly Func<Entity, bool> EntityContainsColor =
			entity => entity.Contains<Color>();

		[Test]
		public void TriggerDoesNotFireWhenItShouldnt()
		{
			var trigger = CreateTriggerWithAction();
			var fired = false;
			trigger.Fired += e => fired = true;
			var entity = new EmptyEntity().AddTrigger(trigger);
			EntitySystem.Current.Run();
			Assert.IsFalse(entity.Contains<Color>());
			Assert.IsFalse(fired);
		}

		private static Trigger CreateTriggerWithAction()
		{
			var trigger = new Trigger(EntityContainsColor);
			trigger.Fired += entity => entity.Set(Color.Red);
			return trigger;
		}

		[Test]
		public void TriggerFiresWhenItShould()
		{
			var trigger = CreateTriggerWithAction();
			var fired = false;
			trigger.Fired += e => fired = true;
			var entity = new EmptyEntity().AddTrigger(trigger).Add(new Color());
			EntitySystem.Current.Run();
			Assert.AreEqual(Color.Red, entity.Get<Color>());
			Assert.IsTrue(fired);
		}

		[Test]
		public void TriggerFiringWithNoActionsIsOk()
		{
			var trigger = new Trigger(EntityContainsColor);
			new EmptyEntity().AddTrigger(trigger).Add(new Color());
			EntitySystem.Current.Run();
		}
	}
}