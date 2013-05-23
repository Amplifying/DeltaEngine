using System;
using System.Text;
using SlimDX.Direct3D9;
using SlimD3D9 = SlimDX.Direct3D9;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXShader : IDisposable
	{
		public SlimDXShader(SlimDXDevice device, string shadersSourceCode)
		{
			this.device = device.Device;
			CreateVertexShader(shadersSourceCode);
			CreatePixelShader(shadersSourceCode);	
		}

		private readonly SlimD3D9.Device device;
		public float[] WorldViewProjectionMatrix;

		private void CreateVertexShader(string shadersSourceCode)
		{
			var bytecode = ShaderBytecode.Compile(Encoding.UTF8.GetBytes(shadersSourceCode), "VS",
				"vs_2_0", ShaderFlags.None);
			vertexShader = new VertexShader(device, bytecode);
		}

		private VertexShader vertexShader;

		private void CreatePixelShader(string shadersSourceCode)
		{
			var bytecode = ShaderBytecode.Compile(Encoding.UTF8.GetBytes(shadersSourceCode), "PS",
				"ps_2_0", ShaderFlags.None);
			pixelShader = new PixelShader(device, bytecode);
		}

		private PixelShader pixelShader;

		public void Apply()
		{
			device.VertexShader = vertexShader;
			device.PixelShader = pixelShader;
			device.SetVertexShaderConstant(0, WorldViewProjectionMatrix);
		}

		public void Dispose()
		{
			pixelShader.Dispose();
			vertexShader.Dispose();
		}
	}
}