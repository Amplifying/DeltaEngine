namespace DeltaEngine.Graphics.OpenTK20
{
	/// <summary>
	/// Vertex and index buffers are either static for mesh drawing, dynamic for changing data or
	/// steam for CircularBuffer (changing every frame).
	/// </summary>
	public enum OpenTK20BufferMode
	{
		/// <summary>
		/// 1-to-n update-to-draw. Data is specified once during initialization (3D Meshes).
		/// </summary>
		Static,
		/// <summary>
		/// N-to-n update-to-draw. Data is drawn multiple times before it changes (Terrains, Custom).
		/// </summary>
		Dynamic,
		/// <summary>
		/// 1-to-1 update-to-draw. Data is very volatile and will change every frame (CircularBuffer).
		/// </summary>
		Stream,
	}
}