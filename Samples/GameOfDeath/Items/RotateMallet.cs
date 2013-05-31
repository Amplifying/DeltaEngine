using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Handles the angle of rotation of a Game of Death Mallet
	/// </summary>
	public class RotateMallet : EntityHandler
	{
		public override void Handle(List<Entity> entities)
		{
			foreach (Mallet mallet in entities.OfType<Mallet>())
				UpdateRotation(mallet);
		}

		private static void UpdateRotation(Mallet mallet)
		{
			var rotationAdjust = RotationSpeed * Time.Current.Delta;
			if (mallet.Rotation < 0 - rotationAdjust)
				mallet.Rotation += rotationAdjust;
		}

		private const float RotationSpeed = 300;

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}