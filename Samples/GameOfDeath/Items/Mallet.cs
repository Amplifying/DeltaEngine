using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Small default weapon to kill single rabbits before they multiply. Not effective to kill masses
	/// </summary>
	public class Mallet : Item
	{
		public Mallet(Content content)
			: base(
				content.Load<Image>("Mallet"), content.Load<Image>("MalletEffect"),
				content.Load<Sound>("MalletSwing")) {}

		public override void UpdatePosition(Point newPosition)
		{
			base.UpdatePosition(newPosition + MalletOffset);
		}

		private static readonly Point MalletOffset = new Point(0.035f, 0.005f);

		protected override float ImpactSize
		{
			get { return 0.035f; }
		}

		protected override float ImpactTime
		{
			get { return 0.0001f; }
		}

		protected override float Damage
		{
			get { return DefaultDamage; }
		}

		internal const float DefaultDamage = 3.3f;

		protected override float DoDamageEvery
		{
			get { return 0.0f; }
		}

		public override int Cost
		{
			get { return 1; }
		}

		public override ItemEffect CreateEffect(Point position)
		{
			if (Rotation < -10)
				return null;

			Rotation = -40;
			return base.CreateEffect(position);
		}

		protected override void Render(Renderer renderer, Time time)
		{
			var rotSpeed = RotationSpeed * time.CurrentDelta;
			if (Rotation < 0 - rotSpeed)
				Rotation += rotSpeed;

			base.Render(renderer, time);
		}

		private const float RotationSpeed = 300;
	}
}