using System.Linq;

namespace DeltaEngine.Graphics
{
	public class VertexFormat
	{
		public VertexFormat(VertexElement[] elements)
		{
			Elements = elements;
			foreach (var vertexElement in elements)
				ComputeElementOffset(vertexElement);
		}

		public VertexElement[] Elements { get; private set; }

		private void ComputeElementOffset(VertexElement vertexElement)
		{
			vertexElement.Offset = Stride;
			Stride += vertexElement.Size;
		}

		public int Stride { get; private set; }

		public VertexElement GetElementFromType(VertexElementType type)
		{
			return Elements.FirstOrDefault(vertexElement => vertexElement.ElementType == type);
		}
	}
}
