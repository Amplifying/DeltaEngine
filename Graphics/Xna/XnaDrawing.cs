using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaGraphics = Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaDrawing : Drawing
	{
		public XnaDrawing(XnaDevice device, Window window)
			: base(device)
		{
			this.device = device;
			this.window = window;
			window.ViewportSizeChanged += Reset;
		}

		private void Reset(Size obj)
		{
			lastTexture = null;
			lastIndices = null;
		}

		private new readonly XnaDevice device;
		private readonly Window window;

		public override void Dispose()
		{
			if (basicEffect != null)
				basicEffect.Dispose();
		}

		public override void DisableTexturing()
		{
			lastTexture = null;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			CheckCreatePositionColorTextureBuffer();
			positionColorUvVertexBuffer.SetData(vertices, 0, vertices.Length, SetDataOptions.Discard);
			BindVertexBuffer(positionColorUvVertexBuffer);
			if (lastTexture != device.NativeDevice.Textures[0])
			{
				ApplyEffect(true);
				lastTexture = device.NativeDevice.Textures[0];
			}
			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length);
			else
				DoDrawIndexed(mode, vertices.Length, lastIndicesCount);
		}

		private Texture lastTexture;

		private void CheckCreatePositionColorTextureBuffer(int vertexCount = 8192)
		{
			if (positionColorUvVertexBuffer != null)
				return;
			positionColorUvVertexBuffer = new DynamicVertexBuffer(device.NativeDevice,
				typeof(XnaGraphics.VertexPositionColorTexture), vertexCount, BufferUsage.WriteOnly);
		}

		private DynamicVertexBuffer positionColorUvVertexBuffer;

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			CheckCreatePositionColorBuffer();
			device.NativeDevice.SetVertexBuffers(null);
			positionColorVertexBuffer.SetData(vertices);
			BindVertexBuffer(positionColorVertexBuffer);
			ApplyEffect(true);
			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length);
			else
				DoDrawIndexed(mode, vertices.Length, lastIndicesCount);
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			CheckCreateIndexBuffer();
			if (lastIndices == indices)
				return;

			if (device.NativeDevice.Indices == indexBuffer)
				device.NativeDevice.Indices = null;
			indexBuffer.SetData(indices);
			device.NativeDevice.Indices = indexBuffer;
			lastIndices = indices;
			lastIndicesCount = usedIndicesCount;
		}

		private short[] lastIndices;
		private int lastIndicesCount = -1;

		private void BindVertexBuffer(VertexBuffer vertexBuffer)
		{
			device.NativeDevice.SetVertexBuffer(vertexBuffer);
		}

		private void ApplyEffect(bool vertexColorEnabled)
		{
			InitializeBasicEffectIfRequired();
			basicEffect.VertexColorEnabled = vertexColorEnabled;
			CheckEnableEffectTexture();
			basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		private void InitializeBasicEffectIfRequired()
		{
			if (basicEffect != null)
				return;
			
			basicEffect = new BasicEffect(device.NativeDevice);
			UpdateProjectionMatrix(window.ViewportPixelSize);
			window.ViewportSizeChanged += UpdateProjectionMatrix;
			FixHalfPixelOffset();
			device.NativeDevice.BlendState = BlendState.NonPremultiplied;
		}

		private BasicEffect basicEffect;

		private void UpdateProjectionMatrix(Size newViewportSize)
		{
			basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, newViewportSize.Width,
				newViewportSize.Height, 0, 0, 1);
		}

		private void FixHalfPixelOffset()
		{
			basicEffect.View = Matrix.CreateTranslation(-0.5f, -0.5f, 0.0f);
		}

		private void CheckEnableEffectTexture()
		{
			if (device.NativeDevice.Textures[0] == null)
				basicEffect.TextureEnabled = false;
			else
			{
				basicEffect.TextureEnabled = true;
				basicEffect.Texture = device.NativeDevice.Textures[0] as Texture2D;
			}
		}

		private void DoDraw(VerticesMode mode, int verticesCount)
		{
			var primitiveMode = Convert(mode);
			var primitiveCount = GetPrimitiveCount(verticesCount, primitiveMode);
			device.NativeDevice.DrawPrimitives(primitiveMode, 0, primitiveCount);
		}

		private void DoDrawIndexed(VerticesMode mode, int verticesCount, int indicesCount)
		{
			var primitiveMode = Convert(mode);
			var primitiveCount = GetPrimitiveCount(indicesCount, primitiveMode);
			device.NativeDevice.DrawIndexedPrimitives(primitiveMode, 0, 0, verticesCount, 0,
				primitiveCount);
		}
		
		private void CheckCreatePositionColorBuffer(int vertexCount = 400)
		{
			if (positionColorVertexBuffer != null)
				return;

			positionColorVertexBuffer = new DynamicVertexBuffer(device.NativeDevice,
				typeof(XnaGraphics.VertexPositionColor), vertexCount, BufferUsage.WriteOnly);
		}

		private DynamicVertexBuffer positionColorVertexBuffer;
		
		private void CheckCreateIndexBuffer(int indexCount = 600)
		{
			if (indexBuffer != null)
				return;
			indexBuffer = new DynamicIndexBuffer(device.NativeDevice, IndexElementSize.SixteenBits,
				indexCount, BufferUsage.WriteOnly);
		}

		private DynamicIndexBuffer indexBuffer;

		private static PrimitiveType Convert(VerticesMode mode)
		{
			switch (mode)
			{
			case VerticesMode.Lines:
				return PrimitiveType.LineList;
			default:
				return PrimitiveType.TriangleList;
			}
		}

		private static int GetPrimitiveCount(int numVerticesOrIndices, PrimitiveType primitiveType)
		{
			switch (primitiveType)
			{
			case PrimitiveType.LineList:
				return numVerticesOrIndices / 2;
			default:
				return numVerticesOrIndices / 3;
			}
		}
	}
}