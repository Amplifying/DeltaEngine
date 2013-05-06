using System;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace DeltaEngine.Graphics.SharpDX
{
	/// <summary>
	/// Base simple shader
	/// </summary>
	public abstract class SharpDXShader : IDisposable
	{
		protected SharpDXShader(SharpDXDevice device)
		{
			this.device = device;
			CreateConstantBuffer();
		}

		protected readonly SharpDXDevice device;

		private void CreateConstantBuffer()
		{
			constantBuffer = new SharpDXBuffer(device.NativeDevice, 64, BindFlags.ConstantBuffer);
		}

		private Buffer constantBuffer;

		protected void CreateVertexShader(byte[] byteCode)
		{
			vertexShader = new VertexShader(device.NativeDevice, byteCode);
		}

		protected void CreatePixelShader(byte[] byteCode)
		{
			pixelShader = new PixelShader(device.NativeDevice, byteCode);
		}

		protected abstract void CreateInputLayout();

		private VertexShader vertexShader;
		private PixelShader pixelShader;
		protected InputLayout inputLayout;

		public void Apply()
		{
			if (isConstantBufferDirty)
				UpdateConstantBuffer();

			var context = device.NativeDevice.ImmediateContext;
			context.VertexShader.Set(vertexShader);
			context.VertexShader.SetConstantBuffer(0, constantBuffer);
			context.PixelShader.Set(pixelShader);
			context.PixelShader.SetConstantBuffer(0, constantBuffer);
			context.InputAssembler.InputLayout = inputLayout;
		}

		private void UpdateConstantBuffer()
		{
			isConstantBufferDirty = false;
			device.SetData(constantBuffer, new[] { worldViewProj }, 1);
		}

		private Matrix worldViewProj = Matrix.Identity;
		private bool isConstantBufferDirty = true;
		public Matrix WorldViewProjection
		{
			set
			{
				worldViewProj = value;
				worldViewProj.Transpose();
				isConstantBufferDirty = true;
			}
		}

		public void Dispose()
		{
			vertexShader.Dispose();
			constantBuffer.Dispose();
			inputLayout.Dispose();
		}
	}
}