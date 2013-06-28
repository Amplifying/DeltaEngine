using Pencil.Gaming.Graphics;
using System;

namespace DeltaEngine.Graphics.GLFW
{
	class GLFWGeometry : Geometry
	{
		public GLFWGeometry()
		{
			bufferUsage = BufferUsageHint.StaticDraw;
		}

		private readonly BufferUsageHint bufferUsage;

		public override void CreateFrom(GeometryData data)
		{
			GeometryData = data;
			CreateVertexBuffer();
			CreateIndexBuffer();
		}

		public GeometryData GeometryData { get; private set; }

		void CreateVertexBuffer()
		{
			GL.GenBuffers(1, out vertexBufferHandle);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(
				GeometryData.NumberOfVertices * GeometryData.Format.Stride),
				GeometryData.VertexDataStream.ToArray(), bufferUsage);
		}

		private int vertexBufferHandle;

		void CreateIndexBuffer()
		{
			GL.GenBuffers(1, out indexBufferHandle);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
			GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(
				GeometryData.NumberOfIndices * sizeof(int)), GeometryData.Indices, bufferUsage);
		}

		private int indexBufferHandle;

		public override void Draw()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
			foreach (var element in GeometryData.Format.Elements)
				BindVertexData(element);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
			GL.DrawElements(BeginMode.Triangles, GeometryData.NumberOfIndices,
				DrawElementsType.UnsignedInt, 0);
		}

		private void BindVertexData(VertexElement element)
		{
			if (element.ElementType == VertexElementType.Position3D)
				BindVertexPosition(element);
			if (element.ElementType == VertexElementType.TextureUV)
				BindVertexTextureUV(element);
			if (element.ElementType == VertexElementType.Color)
				BindVertexColor(element);
		}

		private void BindVertexColor(VertexElement element)
		{
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.ColorPointer(element.ComponentCount, ColorPointerType.UnsignedByte,
				GeometryData.Format.Stride, element.Offset);
		}

		private void BindVertexTextureUV(VertexElement element)
		{
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.TexCoordPointer(element.ComponentCount, TexCoordPointerType.Float,
				GeometryData.Format.Stride, element.Offset);
		}

		private void BindVertexPosition(VertexElement element)
		{
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.VertexPointer(element.ComponentCount, VertexPointerType.Float,
				GeometryData.Format.Stride, element.Offset);
		}

		protected override void DisposeData()
		{
			GeometryData.Dispose();
			GL.DeleteBuffers(1, ref vertexBufferHandle);
			GL.DeleteBuffers(1, ref indexBufferHandle);
		}
	}
}
