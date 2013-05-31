using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Particels
{
	class ParticleEmitterData
	{
		public Image Image { get; set; }
		public Point Position { get; set; }
		public int SpawnIntervalMs { get; set; }
		public long LastSpawnMs { get; set; }
		public int ParticlesCreated { get; set; }
		public float LifeTime { get; set; }
		public ParticlePreset PresetLowerBounds { get; set; }
		public ParticlePreset PresetUpperBounds { get; set; }
	}
}
