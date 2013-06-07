using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Handles the angle of rotation of a Game of Death Mallet
	/// </summary>
	public class RotateMallet : EntityHandler
	{
		public override void Handle(Entity entity)
		{
			var rotation = entity.Get<float>();
			var rotationAdjust = RotationSpeed * Time.Current.Delta;
			if (rotation < 0 - rotationAdjust)
				entity.Set(rotation + rotationAdjust);
		}

		private const float RotationSpeed = 300;

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}