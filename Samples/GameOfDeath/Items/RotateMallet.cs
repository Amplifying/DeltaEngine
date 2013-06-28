using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Handles the angle of rotation of a Game of Death Mallet
	/// </summary>
	public class RotateMallet : Behavior2D
	{
		public override void Handle(Entity2D entity)
		{
			var rotation = entity.Rotation;
			var rotationAdjust = RotationSpeed * Time.Current.Delta;
			if (rotation < 0 - rotationAdjust)
				entity.Rotation = rotation + rotationAdjust;
		}

		private const float RotationSpeed = 300;

		public override Priority Priority
		{
			get { return Priority.First; }
		}
	}
}