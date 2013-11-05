using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Particles;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ParticleSystemPreviewer : ContentPreview
	{
		public override void Preview(string contentName)
		{
			var particleSystemData = ContentLoader.Load<ParticleSystemData>(contentName);
			var system = new ParticleSystem(particleSystemData);
			system.Position = new Vector3D(0.5f, 0.5f, 0);
		}
	}
}