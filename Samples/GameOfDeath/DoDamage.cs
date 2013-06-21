using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;
using GameOfDeath.Items;

namespace GameOfDeath
{
	/// <summary>
	/// Applies the damage from an item if it is time to do so
	/// </summary>
	public class DoDamage : EntityHandler
	{
		public override void Handle(Entity entity)
		{
			var itemEffect = entity as ItemEffect;
			if (itemEffect.Visibility == Visibility.Show &&
				Time.Current.CheckEvery(itemEffect.DamageInterval))
				itemEffect.DoDamage();
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}