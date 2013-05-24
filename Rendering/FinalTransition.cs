using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// This is a transition which removes the Entity from the EntitySystem on completion
	/// </summary>
	public class FinalTransition : Transition
	{
		protected override void EndTransition(Entity entity)
		{
			entity.IsActive = false;
		}
	}
}