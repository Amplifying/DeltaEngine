namespace DeltaEngine.Content
{
	/// <summary>
	/// Content type for ContentData, for details see http://old.deltaengine.net/Wiki.ContentFormats
	/// </summary>
	public enum ContentType
	{
		/// <summary>
		/// .DeltaScene binary file, created by the SceneEditor, loaded when opening scenes.
		/// </summary>
		Scene = 0,

		/// <summary>
		/// .DeltaUIScreen contains all the UI controls positions and values. Scenes have screens.
		/// </summary>
		UIScreen = 1,

		/// <summary>
		/// .DeltaUITheme is always used for scenes and controls to set a style for all buttons, etc.
		/// </summary>
		UITheme = 2,

		/// <summary>
		/// .DeltaLevel is a collection of used meshes their world matrix and maybe some other content
		/// like camera paths, animations, etc. (different level settings for each game).
		/// </summary>
		Level = 3,

		/// <summary>
		/// Image content type, imported as .png bitmaps. Typically stored as compressed textures.
		/// </summary>
		Image = 4,

		/// <summary>
		/// Parent type for images in a sequence, e.g. "Explosion" uses "Explosion01", "Explosion02",
		/// etc. and some extra meta data for displaying this animation at certain speeds.
		/// </summary>
		ImageAnimation = 5,

		/// <summary>
		/// .DeltaShader, which contains platform specific shader data.
		/// </summary>
		Shader = 6,

		/// <summary>
		/// Material, usually used for meshes and advanced rendering classes like Particle Effects.
		/// </summary>
		Material = 7,

		/// <summary>
		/// .DeltaMesh 3D content, either imported from an FBX file (or 3ds, obj, dxf, collada). The
		/// mesh itself is stored just as binary data, use the MeshData class to access it.
		/// </summary>
		Mesh = 8,

		/// <summary>
		/// Animation data for a mesh, e.g. an "Idle", a "Run" or an "Attack" animation of a character. 
		/// </summary>
		MeshAnimation = 9,

		/// <summary>
		/// 3D Models contain no own content, but just links up a bunch of meshes with their used
		/// materials and optionally their MeshAnimations into an easy to use form.
		/// </summary>
		Model = 10,

		/// <summary>
		/// .DeltaParticleEffect represent the content of a particle effect system. Use the Particle
		/// Effect Editor to edit particle effects.
		/// </summary>
		ParticleEffect = 11,

		/// <summary>
		/// Font content, just a .xml file, which was generated from a true type font.
		/// </summary>
		Font = 12,

		/// <summary>
		/// Camera content, stores the initial camera position, rotation and values plus optionally a
		/// camera path this camera should follow.
		/// </summary>
		Camera = 13,

		/// <summary>
		/// Sound file for sound playback, just a .wav file on most platforms.
		/// </summary>
		Sound = 14,

		/// <summary>
		/// Music file for playing in the background or even use streaming. .mp3 or .ogg file.
		/// </summary>
		Music = 15,

		/// <summary>
		/// Video file for multimedia. Supports .mp4 and other video formats.
		/// </summary>
		Video = 16,

		/// <summary>
		/// Helper content type for .DeltaCollision file types. Can be used for both 3D collision meshes
		/// and for 2D collision meshes in Physics. 3D Collision meshes are usually imported from a low
		/// polygon variation of a model. 2D Collisions are imported from images (detecting outlines).
		/// </summary>
		PhysicsCollision = 17,

		/// <summary>
		/// Xml files for game specific content or whatever else you need.
		/// </summary>
		Xml = 18,

		/// <summary>
		/// Json files for game specific content, if you are a JavaScript freak.
		/// </summary>
		Json = 18,

		/// <summary>
		/// You can store trigger data here directly and use it with Actors in Levels.
		/// </summary>
		Trigger = 20,

		/// <summary>
		/// You can store all actor properties dynamically here and reconstruct them when loading!
		/// </summary>
		Actor = 21,

		/// <summary>
		/// Just store the file entry, this can be used for any file (binary makes most sense). All the
		/// logic to load and use this file has to be done by the application.
		/// </summary>
		JustStore = 29,

		/// <summary>
		/// Unsupported Content, shall always be ignored!
		/// </summary>
		Unsupported = 99
	}
}