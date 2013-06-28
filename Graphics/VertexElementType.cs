namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Vertex element. Currently used to dynamically create vertex data,
	/// flags are combined to create a specific format. Data is always aligned
	/// in the same order as specified here. Usage for channels (e.g. texture
	/// channel 0, 1, etc.) is defined by number of UVs used (TexturedUV is
	/// usually in channel 0, then whatever UV comes next is channel 1, etc.).
	/// Note: All the byte sizes should be kept in sync with the VertexFormat
	/// GetVertexDataLength helper method!
	/// </summary>
	public enum VertexElementType
	{
		/// <summary>
		/// 2D position data (Point, 2 floats = 8 bytes).
		/// If compressed form (2 shorts = 4 bytes, ranging from
		/// 0 to 16k instead of 0.0-1.0, this way we have enough values to go from
		/// -2.0 to +2.0, which is good enough for 2D UI).
		/// Note: The ScreenSpace.ViewProjection2DViaShorts must be scaled correctly to
		/// account for this scaling (1.0f/16384.0f) before rendering.
		/// </summary>
		Position2D,

		/// <summary>
		/// 3D position data (Vector, 3 floats = 12 bytes). Note: Often used in
		/// combination with the other compressed data types because it is just
		/// easier to use and does not require handling in the vertex shader
		/// (e.g. for ES 1.1 or WP7 we got no shaders).
		/// If compressed form (3 shorts = 6 bytes). Each short
		/// is ranging from -32k to +32k and needs to be scaled with the world
		/// matrix of the model to account for the scaling factor needed. This
		/// scaling is based on VertexData.Position3DCompressedMultiplier, which
		/// is 1/256.0f, which allows 3D positions from -128.0f to +128.0f.
		/// Note: After Position3DCompressed the offset is 6 bytes, which is NOT
		/// 4 byte aligned, thus the next data type should be short, not float,
		/// else our performance will be killed (just use only Compressed).
		/// </summary>
		Position3D,

		/// <summary>
		/// Normal vector data for light calculation and normal mapping (Vector,
		/// 3 floats = 12 bytes). If compressed, then just 3 normalized bytes are
		/// used +1 byte padding (because it all has to be 4 byte aligned).
		/// </summary>
		Normal,

		/// <summary>
		/// Tangent vector for normal mapping (Vector, 3 floats = 12 bytes).
		/// If compressed, then just 3 normalized bytes are used +1 byte padding
		/// (because it all has to be 4 byte aligned).
		/// </summary>
		Tangent,

		/// <summary>
		/// Binormal vector for normal mapping (Vector, 3 floats = 12 bytes).
		/// If compressed, then just 3 normalized bytes are used +1 byte padding
		/// (because it all has to be 4 byte aligned).
		/// </summary>
		Binormal,

		/// <summary>
		/// Color for this vertex (Color, 4 bytes).
		/// </summary>
		Color,

		/// <summary>
		/// UV data (Point, 2 floats = 8 bytes)
		/// Compressed UV data as shorts (2 shorts = 4 bytes), while the data is
		/// very small, it needs to be converted from shorts to real UVs for use
		/// in the vertex shader (done automatically in OpenGL ES via normalize,
		/// 1/32k is the scaling, see VertexData.TextureUVCompressedMultiplier).
		/// </summary>
		TextureUV,

		/// <summary>
		/// UVW 3D texture data for cube mapping or reflection cube maps
		/// (Vector, 3 floats = 12 bytes).
		/// UVW 3D texture data in compressed form for cube mapping or reflection
		/// cube maps (3 shorts = 6 bytes), same rules as for TextureUVCompressed
		/// apply here too (1/32k automatically performed in OpenGL ES).
		/// </summary>
		TextureUVW,

		/// <summary>
		/// Optional light map UV channel (secondary texture channel). Please note
		/// that most texture channels all use the same TextureUV data (e.g.
		/// DiffuseMap, NormalMap and SpecularMap), but LightMap has extra UVs.
		/// Uses Point, 2 floats = 8 bytes.
		/// Optional light map UV channel in compressed form, same rules as for
		/// TextureUVCompressed apply here (2 shorts = 4 bytes, 1/32k scaling).
		/// </summary>
		LightMapUV,

		/// <summary>
		/// Extra UV channel for crazy purposes :D (Point, 2 floats = 8 bytes).
		/// Compressed form, same rules as for
		/// TextureUVCompressed apply here (2 shorts = 4 bytes, 1/32k scaling).
		/// </summary>
		ExtraUV,

		/// <summary>
		/// Skin indices as compressed data for the skin weights, just 2 shorts,
		/// should be used together with SkinWeight for 4 byte alignment. This
		/// could also be optimized to 2 bytes, but then our vertex data is not
		/// longer 4 byte aligned (which is not allowed in DirectX/XNA and can
		/// sometimes be bad for performance in OpenGL, but usually supported).
		/// Note: Unlike most other vertex element flags this one has no
		/// compressed counterpart and is not normalized, the format is fine.
		/// </summary>
		SkinIndices,

		/// <summary>
		/// Skin weight data for skinning (just 2 normalized shorts = 4 bytes).
		/// Will be automatically normalized when passed to the vertex shader.
		/// </summary>
		SkinWeights,
	}
}
