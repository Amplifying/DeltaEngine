namespace DeltaEngine.Graphics.SlimDX
{
	public struct SlimDXShadersSourceCode
	{
		public static string PositionColor =
			"float4x4 WorldViewProjection;" +

			"struct VS_OUTPUT" +
			"{" +
				"float4 Pos       : POSITION;" +
				"float4 Color     : COLOR0;" +
			"};" +

			"VS_OUTPUT VS( float3 Pos : POSITION, float4 Color : COLOR )" +
			"{" +
				"VS_OUTPUT output = (VS_OUTPUT)0;" +
				"output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);" +
				"output.Color = float4(Color[2], Color[1], Color[0], Color[3]);" +
				"return output;" +
			"}" +

			"float4 PS( VS_OUTPUT input ) : COLOR0" +
			"{" +
				"return input.Color;" +
			"}";

		public static string PositionColorTexture =
			"float4x4 WorldViewProjection;" +

			"struct VS_OUTPUT" +
			"{" +
				"float4 Pos       : POSITION;" +
				"float4 Color     : COLOR0;" +
				"float2 TextureUV : TEXCOORD0;" +
			"};" +

			"VS_OUTPUT VS( float3 Pos : POSITION, float4 Color : COLOR, float2 TextureUV : TEXCOORD0 )" +
			"{" +
				"VS_OUTPUT output = (VS_OUTPUT)0;" +
				"output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);" +
				"output.Color = float4(Color[2], Color[1], Color[0], Color[3]);" +
				"output.TextureUV = TextureUV;" +
				"return output;" +
			"}" +

			"sampler DiffuseTexture;" +

			"float4 PS( VS_OUTPUT input ) : COLOR0" +
			"{" +
				"return tex2D(DiffuseTexture, input.TextureUV) * input.Color;" +
			"}";
	}
}