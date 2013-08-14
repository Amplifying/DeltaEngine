using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Particles;

namespace $safeprojectname$
{
	public class ParticleFountain : ParticleEmitter
	{
		public ParticleFountain(Point position) : base(EmitterData, position)
		{
			CreateCommands();
		}

		private static ParticleEmitterCreator EmitterData
		{
			get
			{
				return emitterData = new ParticleEmitterCreator {
					StartVelocity = new Point(0.0f, -1.0f),
					Force = new Point(0, 0.9f),
					LifeTime = 1.0f,
					MaximumNumberOfParticles = 512,
					Size = new Range<Size>(new Size(0.01f), new Size(0.015f)),
					ParticleMaterial = new Material(Shader.Position2DColorUv, "Particle"),
					SpawnInterval = 0.01f,
					StartColor = Color.Red,
					StartVelocityVariance = new Point(0.5f, 0.1f)
				};
			}
		}

		private static ParticleEmitterCreator emitterData;

		private static void CreateCommands()
		{
			new Command(() => 
			{
				emitterData.StartVelocity += new Point(0, -0.3f) * Time.Delta;
			}).Add(new KeyTrigger(Key.CursorUp, State.Pressed));
			new Command(() => 
			{
				emitterData.StartVelocity += new Point(0, 0.3f) * Time.Delta;
			}).Add(new KeyTrigger(Key.CursorDown, State.Pressed));
			new Command(() => 
			{
				emitterData.Force += new Point(0, -0.3f) * Time.Delta;
			}).Add(new KeyTrigger(Key.CursorLeft, State.Pressed));
			new Command(() => 
			{
				emitterData.Force += new Point(0, 0.3f) * Time.Delta;
			}).Add(new KeyTrigger(Key.CursorRight, State.Pressed));
		}
	}
}