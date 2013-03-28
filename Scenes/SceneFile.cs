using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Loads and saves a scene to file
	/// </summary>
	public class SceneFile
	{
		public SceneFile(Scene scene)
		{
			Scene = scene;
		}

		public Scene Scene { get; private set; }

		public SceneFile(string filePath)
		{
			if (Path.GetExtension(filePath) == "")
				filePath += ".dat";

			using (var s = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new BinaryReader(s))
				Scene = reader.Create<Scene>();
		}

		public void Save(string filePath)
		{
			if (Path.GetExtension(filePath) == "")
				filePath += ".dat";

			using (
				var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
					FileShare.ReadWrite))
			using (var writer = new BinaryWriter(stream))
				Scene.Save(writer);
		}
	}
}