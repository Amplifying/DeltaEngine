using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Particles;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	public class ContentDisplayChangerTests : TestWithMocksOrVisually
	{
		[Test]
		public void MoveAParticle()
		{
			var emptyMaterial = new Material(Shader.Position2DColor, "");
			var emitter =
				new ParticleEmitter(new ParticleEmitterData { ParticleMaterial = emptyMaterial },
					Vector2D.Zero);
			ContentDisplayChanger.MoveParticle(new Vector2D(1.5f, 1.5f), emitter);
			ResetPositions();
		}

		private void ResetPositions()
		{
			ContentDisplayChanger.lastPanPosition = new Vector2D(0, 0);
			ContentDisplayChanger.lastScalePosition = new Vector2D(0, 0);
			ContentDisplayChanger.lastRotatePosition = new Vector2D(0, 0);
			ContentDisplayChanger.lastZoomPosition = new Vector2D(0, 0);
		}
	}
}