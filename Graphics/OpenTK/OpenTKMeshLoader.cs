using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;

namespace DeltaEngine.Graphics.OpenTK
{
	public class OpenTKMeshLoader
	{
		public OpenTKMeshLoader(Stream fileData)
		{
			this.fileData = new StreamReader(fileData);
			LoadMesh();
		}

		private readonly TextReader fileData;

		private void LoadMesh()
		{
			CreateObjects();
			ParseFile();
		}

		private void CreateObjects()
		{
			Positions = new List<Vector4>();
			Normals = new List<Vector3>();
			TexCoords = new List<Vector2>();
			Indices = new List<int>();
			dictionary = new Dictionary<ObjVertex, int>();
			vertices = new List<ObjVertex>();
		}

		public List<Vector4> Positions { get; private set; }
		public List<Vector3> Normals { get; private set; }
		public List<Vector2> TexCoords { get; private set; }
		public List<int> Indices { get; private set; }

		[StructLayout(LayoutKind.Sequential)]
		public struct ObjVertex
		{
			public Vector4 Position;
			public Vector2 TexCoord;
			public Vector3 Normal;
		}

		private Dictionary<ObjVertex, int> dictionary;
		private List<ObjVertex> vertices;

		private void ParseFile()
		{
			string line;
			while ((line = fileData.ReadLine()) != null)
			{
				line = line.Trim();
				ParseLine(line.Split(' '));
			}
		}

		private void ParseLine(string[] values)
		{
			switch (values[0])
			{
				case "v":
					ReadPosition(values);
					break;
				case "vn":
					ReadNormal(values);
					break;
				case "vt":
					ReadTextureCoordinate(values);
					break;
				case "f":
					ReadIndice(values);
					break;
			}			
		}

		private void ReadPosition(string[] values)
		{
			float x = float.Parse(values[1]);
			float y = float.Parse(values[2]);
			float z = float.Parse(values[3]);
			Positions.Add(new Vector4(x, y, z, 0.0f));
		}

		private void ReadNormal(string[] values)
		{
			float x = float.Parse(values[1]);
			float y = float.Parse(values[2]);
			float z = float.Parse(values[3]);
			Normals.Add(new Vector3(x, y, z));
		}

		private void ReadTextureCoordinate(string[] values)
		{
			float u = float.Parse(values[1]);
			float v = float.Parse(values[2]);
			TexCoords.Add(new Vector2(u, v));			
		}

		private void ReadIndice(string[] values)
		{
			for (int i = 1; i < values.Length; i++)
				Indices.Add(ParseFaceParameter(values[i]));
		}

		private int ParseFaceParameter(string faceParameter)
		{
			var values = faceParameter.Split('/');
			var position = GetPosition(values);
			var textureCoordinate = GetTextureCoordinate(values);
			var normal = GetNormal(values);
			return FindOrAddVertex(position, textureCoordinate, normal);
		}

		private Vector4 GetPosition(string[] values)
		{
			int index = int.Parse(values[0]);
			index = index < 0 ? Positions.Count + index : index - 1;
			return Positions[index];			
		}

		private Vector2 GetTextureCoordinate(string[] values)
		{
			if (values.Length < 2)
				return new Vector2();

			int index = int.Parse(values[1]);
			index = index < 0 ? TexCoords.Count + index : index - 1;
			return TexCoords[index];
		}

		private Vector3 GetNormal(string[] values)
		{
			if (values.Length < 3)
				return new Vector3();

			int index = int.Parse(values[2]);
			index = index < 0 ? Normals.Count + index : index - 1;
			return Normals[index];
		}

		private int FindOrAddVertex(Vector4 position, Vector2 texCoord, Vector3 normal)
		{
			var vertex = new ObjVertex { Position = position, TexCoord = texCoord, Normal = normal };
			int index;
			if (dictionary.TryGetValue(vertex, out index))
				return index;

			vertices.Add(vertex);
			dictionary[vertex] = vertices.Count - 1;
			return vertices.Count - 1;
		}
	}
}