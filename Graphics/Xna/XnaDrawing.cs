using System;
using System.Reflection;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework.Graphics;
using XnaGraphics = Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;
using XnaVertexPositionColor = Microsoft.Xna.Framework.Graphics.VertexPositionColor;
using XnaVertexPositionColorTextured = Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaDrawing : Drawing
	{
		public XnaDrawing(XnaDevice device, Window window)
			: base(device)
		{
			nativeDevice = device.NativeDevice;
			this.window = window;
			InitializeBuffers();
			InitializeBasicEffect();
		}

		private readonly GraphicsDevice nativeDevice;
		private readonly Window window;

		private void InitializeBuffers()
		{
			positionColorVertexBuffer = new XnaCircularVertexBuffer<VertexPositionColor>(VertexBufferSize,
				typeof(XnaVertexPositionColor), (XnaDevice)device);
			positionColorUvVertexBuffer = new XnaCircularVertexBuffer<VertexPositionColorTextured>(
				VertexBufferSize, typeof(XnaVertexPositionColorTextured), (XnaDevice)device);
			indexBuffer = new XnaCircularIndexBuffer(IndexBufferSize, (XnaDevice)device);
		}

		private XnaCircularVertexBuffer<VertexPositionColor> positionColorVertexBuffer;
		private XnaCircularVertexBuffer<VertexPositionColorTextured> positionColorUvVertexBuffer;
		private XnaCircularIndexBuffer indexBuffer;

		private const int VertexBufferSize = 16384;
		private const int IndexBufferSize = 65536;

		private void InitializeBasicEffect()
		{
			basicEffect = new BasicEffect(nativeDevice);
			UpdateProjectionMatrix(window.ViewportPixelSize);
			window.ViewportSizeChanged += UpdateProjectionMatrix;
			basicEffect.View = Matrix.CreateTranslation(-0.5f, -0.5f, 0.0f);
			nativeDevice.BlendState = BlendState.NonPremultiplied;
		}

		public override void Dispose()
		{
			DisposeBuffers();
			if (basicEffect != null)
				basicEffect.Dispose();
		}

		private BasicEffect basicEffect;

		private void DisposeBuffers()
		{
			if (positionColorVertexBuffer.IsCreated)
				positionColorVertexBuffer.Dispose();

			if (positionColorUvVertexBuffer.IsCreated)
				positionColorUvVertexBuffer.Dispose();

			if (indexBuffer.IsCreated)
				indexBuffer.Dispose();
		}

		public override void EnableTexturing(Image image)
		{
			var nativeTexture = ((XnaImage)image).NativeTexture;
#if DEBUG
			// Check whether the intialization order was correct in AutofacStarter when the Run()
			// method is called. If it was not, the internal pointer in the native texture (pComPtr)
			// would have a null value (0x00) and the next assignment would crash.
			CheckIfTheInitializationOrderInResolverWasCorrect(nativeTexture);
#endif
			nativeDevice.Textures[0] = nativeTexture;
			nativeDevice.SamplerStates[0] = image.DisableLinearFiltering
				? SamplerState.PointClamp : SamplerState.LinearClamp;
			basicEffect.TextureEnabled = true;
			basicEffect.Texture = nativeDevice.Textures[0] as Texture2D;
		}

#if DEBUG
		private void CheckIfTheInitializationOrderInResolverWasCorrect(Texture2D nativeTexture)
		{
			if (!initializationOrderAlreadyChecked)
			{
				initializationOrderAlreadyChecked = true;
				if (NativePointerIsNull(nativeTexture))
					throw new InitializationOrderIsWrongCheckIfInitializationHappensInResolverEvent();
			}
		}

		public class InitializationOrderIsWrongCheckIfInitializationHappensInResolverEvent :
			Exception {}

		private bool initializationOrderAlreadyChecked;

		private static bool NativePointerIsNull(Texture2D nativeTexture)
		{
			var nativePointerField =
				nativeTexture.GetType().GetField("pComPtr", BindingFlags.Instance | BindingFlags.NonPublic);
			var nativePointer = nativePointerField.GetValue(nativeTexture);
			unsafe
			{
				var nativePointerValue = Pointer.Unbox(nativePointer);
				return nativePointerValue == null;
			}
		}
#endif

		public override void DisableTexturing()
		{
			basicEffect.TextureEnabled = false;
		}

		public override void SetBlending(BlendMode blendMode)
		{
			if (currentBlendMode == blendMode)
				return;

			nativeDevice.BlendState = GetXnaBlendState(blendMode);
			currentBlendMode = blendMode;
		}

		private BlendMode currentBlendMode = BlendMode.Opaque;

		private static BlendState GetXnaBlendState(BlendMode blendMode)
		{
			if (blendMode == BlendMode.Additive)
				return BlendState.Additive;

			if (blendMode == BlendMode.Opaque)
				return BlendState.Opaque;

			return BlendState.AlphaBlend;
		}

		public override void DrawVerticesForSprite(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			if (!positionColorUvVertexBuffer.IsCreated)
				positionColorUvVertexBuffer.Create();

			positionColorUvVertexBuffer.SetData(vertices);
			nativeDevice.SetVertexBuffer(positionColorUvVertexBuffer.NativeBuffer);
			Draw(positionColorUvVertexBuffer, mode, vertices.Length);
		}

		private void ApplyEffect()
		{
			basicEffect.VertexColorEnabled = true;
			basicEffect.TextureEnabled = nativeDevice.Textures[0] != null;
			if (basicEffect.TextureEnabled)
				basicEffect.Texture = nativeDevice.Textures[0] as Texture2D;

			basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		private void Draw<T>(XnaCircularVertexBuffer<T> buffer, VerticesMode mode, int length)
			 where T : struct
		{
			ApplyEffect();
			var primitiveMode = Convert(mode);
			var primitiveCount =
				GetPrimitiveCount(lastIndicesCount == -1 ? length : lastIndicesCount, primitiveMode);
			if (lastIndicesCount == -1)
				DrawPrimitives(buffer, primitiveMode, primitiveCount);
			else
				DrawIndexedPrimitives(buffer, primitiveMode, length, primitiveCount);
		}

		private int lastIndicesCount = -1;

		private void DrawPrimitives<T>(XnaCircularVertexBuffer<T> buffer, PrimitiveType primitiveType,
			int primitiveCount) where T : struct
		{
			nativeDevice.DrawPrimitives(primitiveType, buffer.Offset / buffer.VertexSize,
				primitiveCount);
		}

		private void DrawIndexedPrimitives<T>(XnaCircularVertexBuffer<T> buffer,
			PrimitiveType primitiveType, int length, int primitiveCount) where T : struct
		{
			nativeDevice.DrawIndexedPrimitives(primitiveType, buffer.Offset / buffer.VertexSize,
				0, length, 0, primitiveCount);
		}

		private static PrimitiveType Convert(VerticesMode mode)
		{
			return mode == VerticesMode.Triangles ? PrimitiveType.TriangleList : PrimitiveType.LineList;
		}

		private static int GetPrimitiveCount(int numVerticesOrIndices, PrimitiveType primitiveType)
		{
			return primitiveType == PrimitiveType.LineList
				? numVerticesOrIndices / 2 : numVerticesOrIndices / 3;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			if (!positionColorVertexBuffer.IsCreated)
				positionColorVertexBuffer.Create();

			positionColorVertexBuffer.SetData(vertices);
			nativeDevice.SetVertexBuffer(positionColorVertexBuffer.NativeBuffer);
			Draw(positionColorVertexBuffer, mode, vertices.Length);
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			if (!indexBuffer.IsCreated)
				indexBuffer.Create();

			nativeDevice.Indices = null;
			indexBuffer.SetData(indices);
			nativeDevice.Indices = indexBuffer.NativeBuffer;
			lastIndicesCount = usedIndicesCount;
		}

		public override void DisableIndices()
		{
			lastIndicesCount = -1;
		}
	
		private void UpdateProjectionMatrix(Size newViewportSize)
		{
			basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, newViewportSize.Width,
				newViewportSize.Height, 0, 0, 1);
		}
	}
}