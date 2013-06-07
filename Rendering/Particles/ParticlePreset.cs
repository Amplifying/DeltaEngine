using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Particles
{
	/// <summary>
	/// Defines starting values for a particle created by the ParticleEmitter
	/// </summary>
	public class ParticlePreset
	{
		public ParticlePreset() { }

		public ParticlePreset(ParticlePreset preset1, ParticlePreset preset2)
		{
			Position = RandomizeBetweenPoints(preset1.Position, preset2.Position);
			StartVelocity = RandomizeBetweenPoints(preset1.StartVelocity, preset2.StartVelocity);
			Velocity = RandomizeBetweenPoints(preset1.Velocity, preset2.Velocity);
			StartRotation = RandomizeBetweenFloats(preset1.StartRotation, preset2.StartRotation);
			Rotation = RandomizeBetweenFloats(preset1.Rotation, preset2.Rotation);
			StartSize = RandomizeBetweenSizes(preset1.StartSize, preset2.StartSize);
			Size = RandomizeBetweenSizes(preset1.Size, preset2.Size);
			Lifetime = RandomizeBetweenFloats(preset1.Lifetime, preset2.Lifetime);
			Speed = RandomizeBetweenFloats(preset1.Speed, preset2.Speed);
			Color = RandomizeBetweenColors(preset1.Color, preset2.Color);
		}

		public Point Position { get; set; }
		public Point StartVelocity { get; set; }
		public Point Velocity { get; set; }
		public float StartRotation { get; set; }
		public float Rotation { get; set; }
		public Size StartSize { get; set; }
		public Size Size { get; set; }
		public float Lifetime { get; set; }
		public float Speed { get; set; }
		public Color Color { get; set; }

		private static Point RandomizeBetweenPoints(Point point1, Point point2)
		{
			return new Point(RandomizeBetweenFloats(point1.X, point2.X),
				RandomizeBetweenFloats(point1.Y, point2.Y));
		}

		private static Size RandomizeBetweenSizes(Size size1, Size size2)
		{
			return Size.Lerp(size1, size2, Randomizer.Current.Get());
		}

		private static float RandomizeBetweenFloats(float value1, float value2)
		{
			return MathExtensions.Lerp(value1, value2, Randomizer.Current.Get());
		}

		private static Color RandomizeBetweenColors(Color color1, Color color2)
		{
			return Color.Lerp(color1, color2, Randomizer.Current.Get());
		}
	}
}