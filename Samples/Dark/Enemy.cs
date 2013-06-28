using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace Dark
{
	public class EdgeShrinkFactor
	{
		public EdgeShrinkFactor()
			: this(0.0f) { }

		public EdgeShrinkFactor(float value)
		{
			Value = value;
		}

		public float Value { get; set; }
	}

	public class Enemy : Character
	{
		public Enemy(Image image, ContentLoader content, List<Vector> wayPoints)
			: base(image, Vector.Zero)
		{
			State = CharacterState.Moving;
			SetupSpeed();
			this.wayPoints = wayPoints;
			Position = wayPoints[0];
			SelectWayPoint(1);
			Start<RenderShadow>();
			Add(new EdgeShrinkFactor(0.05f));
			for (int i = 1; i <= ImageCount; i++)
				images.Add(ContentLoader.Load<Image>("AsylumBoy112px0" + i));
		}

		private readonly List<Vector> wayPoints;
		private const int ImageCount = 5;

		protected override void SelectImage()
		{
			Image = images[animationStep];
			if (timeSinceLastImageUpdate >= TimePerImage)
			{
				timeSinceLastImageUpdate = 0.0f;
				animationStep++;
				if (animationStep == ImageCount)
					animationStep = 0;
			}
		}

		private void SetupSpeed()
		{
			var rand = new Random();
			Speed = MinSpeed + (float)rand.NextDouble() * SpeedRange;
		}

		private const float MinSpeed = 0.05f;
		private const float SpeedRange = 0.15f;

		private void SelectWayPoint(int wayPointIndex)
		{
			currentWayPoint = wayPointIndex;
			Facing = Vector2DMath.Normalize(wayPoints[currentWayPoint] - Position);
		}

		private int currentWayPoint;

		public override void Update()
		{
			base.Update();
			if (Vector2DMath.GetSquaredLength(wayPoints[currentWayPoint] - Position) < Near)
				SelectWayPoint(currentWayPoint < (wayPoints.Count - 1) ? currentWayPoint + 1 : 0);
		}

		private const float Near = 0.0001f;
	}
}