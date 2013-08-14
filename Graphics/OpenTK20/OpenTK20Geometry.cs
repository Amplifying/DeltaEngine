using System;

namespace DeltaEngine.Graphics.OpenTK20
{
	/// <summary>
	/// GPU geometry data in OpenGL
	/// </summary>
	public class OpenTK20Geometry : Geometry
	{
		public OpenTK20Geometry(string contentName, OpenTK20Device device)
			: base(contentName)
		{
			this.device = device;
		}

		private OpenTK20Geometry(GeometryCreationData creationData, OpenTK20Device device)
			: base(creationData)
		{
			this.device = device;
		}

		private readonly OpenTK20Device device;

		protected override void SetNativeData(byte[] vertexData, short[] indices)
		{
			if (vertexBufferHandle == OpenTK20Device.InvalidHandle)
				CreateBuffers();
			device.LoadVertexData(0, vertexData, vertexData.Length);
			device.LoadIndices(0, indices, indices.Length * sizeof(short));
		}

		private int vertexBufferHandle = OpenTK20Device.InvalidHandle;

		private void CreateBuffers()
		{
			int vertexDataSize = NumberOfVertices * Format.Stride;
			vertexBufferHandle = device.CreateVertexBuffer(vertexDataSize, OpenTK20BufferMode.Static);
			if (vertexBufferHandle == OpenTK20Device.InvalidHandle)
				throw new UnableToCreateOpenGLGeometry();
			indexBufferHandle = device.CreateIndexBuffer(NumberOfIndices * sizeof(short),
				OpenTK20BufferMode.Static);
		}

		private class UnableToCreateOpenGLGeometry : Exception {}

		private int indexBufferHandle = OpenTK20Device.InvalidHandle;

		public override void Draw()
		{
			if (vertexBufferHandle == OpenTK20Device.InvalidHandle)
				throw new UnableToDrawDynamicGeometrySetDataNeedsToBeCalledFirst();
			device.BindVertexBuffer(vertexBufferHandle);
			device.BindIndexBuffer(indexBufferHandle);
			device.DrawTriangles(0, NumberOfIndices);
		}

		private class UnableToDrawDynamicGeometrySetDataNeedsToBeCalledFirst : Exception {}

		protected override void DisposeData()
		{
			if (vertexBufferHandle == OpenTK20Device.InvalidHandle)
				return;
			device.DeleteBuffer(vertexBufferHandle);
			device.DeleteBuffer(indexBufferHandle);
			vertexBufferHandle = OpenTK20Device.InvalidHandle;
		}
	}
}