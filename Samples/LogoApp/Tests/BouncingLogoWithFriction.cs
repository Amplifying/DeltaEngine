using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;

namespace LogoApp.Tests
{
	/// <summary>
	/// Adds friction to the BouncingLogo class so each logo slows down over time.
	/// </summary>
	public class BouncingLogoWithFriction : BouncingLogo
	{
		public BouncingLogoWithFriction(Content content, Randomizer random)
			: base(content, random) {}

		protected override void Render(Renderer renderer, Time time)
		{
			float frictionMultiplier = 1.0f - FrictionFactorPerSecond * time.CurrentDelta;
			velocity *= frictionMultiplier;
			rotationSpeed *= frictionMultiplier;
			Point remVelocity = velocity;
			base.Render(renderer, time);
			if (velocity == remVelocity)
				return;

			frictionMultiplier = 1.0f - FrictionFactorPerSecond * 0.5f;
			velocity *= frictionMultiplier;
		}

		private const float FrictionFactorPerSecond = 0.1f;
	}
}