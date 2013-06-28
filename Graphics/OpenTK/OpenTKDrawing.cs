using System;
using OpenTK.Graphics.OpenGL;

namespace DeltaEngine.Graphics.OpenTK
{
	public sealed class OpenTKDrawing : Drawing
	{
		public OpenTKDrawing(Device device) : base(device)
		{
			InitializeBuffers();
			SetBlending(BlendMode.Normal);
		}

		private BlendMode currentBlendMode = BlendMode.Opaque;
		private OpenTKCircularBuffer<VertexPositionColor> positionColorBuffer;
		private OpenTKCircularBuffer<VertexPositionColorTextured> positionColorUvBuffer;
		private OpenTKCircularBuffer<short> indexBuffer;
		private const int VertexBufferSize = 16384;
		private const int IndexBufferSize = 65536;
		private const int InvalidHandle = -1;
		private int lastIndicesCount = InvalidHandle;
		private const int InitialVertexBufferSize = 8192;

		public override void Dispose()
		{
			if (positionColorBuffer.IsCreated)
				positionColorBuffer.Dispose();

			if (positionColorUvBuffer.IsCreated)
				positionColorUvBuffer.Dispose();

			if (indexBuffer.IsCreated)
				indexBuffer.Dispose();
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			if (!indexBuffer.IsCreated)
				indexBuffer.Create();

			indexBuffer.SetData(indices);
			lastIndicesCount = usedIndicesCount;
		}

		public override void DisableIndices()
		{
			if (indexBuffer.IsCreated)
				indexBuffer.Dispose();
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			if (!positionColorBuffer.IsCreated)
				positionColorBuffer.Create();

			positionColorBuffer.SetData(vertices);
			SetBaseClientStates();
			SetBaseVertexDeclaration(VertexPositionColor.SizeInBytes);
			DecideKindOfDraw(mode, vertices.Length);
			lastIndicesCount = InvalidHandle;
			NumberOfVerticesDrawn += vertices.Length;
			NumberOfTimesDrawn++;
		}

		public override void DrawVerticesForSprite(VerticesMode mode, VertexPositionColorTextured[] 
			vertices)
		{
			if (!positionColorUvBuffer.IsCreated)
				positionColorUvBuffer.Create();

			positionColorUvBuffer.SetData(vertices);
			SetBaseClientStates();
			EnableTextureCoordArray();
			SetBaseVertexDeclaration(VertexPositionColorTextured.SizeInBytes);
			SetTextureCoordDeclaration();
			DecideKindOfDraw(mode, vertices.Length);
			lastIndicesCount = InvalidHandle;
			NumberOfVerticesDrawn += vertices.Length;
			NumberOfTimesDrawn++;
		}

		private void InitializeBuffers()
		{
			positionColorBuffer = new OpenTKCircularBuffer<VertexPositionColor>(VertexBufferSize);
			positionColorUvBuffer = new 
				OpenTKCircularBuffer<VertexPositionColorTextured>(VertexBufferSize);
			indexBuffer = new OpenTKCircularBuffer<short>(IndexBufferSize);
		}

		public override void SetBlending(BlendMode blendMode)
		{
			if (currentBlendMode == blendMode)
				return;

			currentBlendMode = blendMode;
			switch (blendMode)
			{
				case BlendMode.Normal:
					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					GL.BlendEquation(BlendEquationMode.FuncAdd);
					break;
				case BlendMode.Opaque:
					GL.Disable(EnableCap.Blend);
					break;
				case BlendMode.AlphaTest:
					GL.Disable(EnableCap.Blend);
					GL.Enable(EnableCap.AlphaTest);
					break;
				case BlendMode.Additive:
					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
					GL.BlendEquation(BlendEquationMode.FuncAdd);
					break;
				case BlendMode.Subtractive:
					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
					GL.BlendEquation(BlendEquationMode.FuncReverseSubtract);
					break;
				case BlendMode.LightEffect:
					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.One);
					GL.BlendEquation(BlendEquationMode.FuncAdd);
					break;
			}
		}

		public override void EnableTexturing(Image image)
		{
			GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D, (image as OpenTKImage).Handle);
		}

		public override void DisableTexturing()
		{
			GL.Disable(EnableCap.Texture2D);
		}

		private void SetBaseClientStates()
		{
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.VertexArray);
		}

		private void EnableTextureCoordArray()
		{
			GL.EnableClientState(ArrayCap.TextureCoordArray);
		}

		private void SetBaseVertexDeclaration(int stride)
		{
			GL.VertexPointer(3, VertexPointerType.Float, stride, IntPtr.Zero);
			GL.ColorPointer(4, ColorPointerType.UnsignedByte, stride, (IntPtr)12);
		}

		private void SetTextureCoordDeclaration()
		{
			GL.TexCoordPointer(2, TexCoordPointerType.Float, VertexPositionColorTextured.SizeInBytes, 
				(IntPtr)16);
		}

		private void DecideKindOfDraw(VerticesMode mode, int numVertices)
		{
			if (lastIndicesCount == -1)
				GL.DrawArrays(Convert(mode), 0, numVertices);
			else
				GL.DrawElements(Convert(mode), lastIndicesCount, DrawElementsType.UnsignedShort, 
					IntPtr.Zero);
		}

		private static BeginMode Convert(VerticesMode mode)
		{
			switch (mode)
			{
				case VerticesMode.Lines:
					return BeginMode.Lines;
				default:
					return BeginMode.Triangles;
			}
		}
	}
}