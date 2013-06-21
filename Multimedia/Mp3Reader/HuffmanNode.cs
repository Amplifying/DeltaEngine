using System;
using System.Collections.Generic;

namespace Mp3Reader
{
	internal abstract class BaseHuffmanNode<T>
		where T : class, new()
	{
		public T[] Nodes;
		protected int[] BaseData;

		protected BaseHuffmanNode(int size)
		{
			BaseData = new int[size];
			for (int index = 0; index < size; index++)
				BaseData[index] = -1;
		}

		public void Add()
		{
			Nodes = new T[2];
			Nodes[0] = new T();
			Nodes[1] = new T();
		}

		public int[] Data
		{
			get
			{
				for (int index = 0; index < BaseData.Length; index++)
					if (BaseData[0] == -1)
						throw new Exception("Not on end node.");

				return BaseData;
			}
			set { BaseData = value; }
		}
	}

	internal class HuffmanNode : BaseHuffmanNode<HuffmanNode>
	{
		public HuffmanNode()
			: base(2) {}

		public int X
		{
			get { return BaseData[0]; }
			set { BaseData[0] = value; }
		}

		public int Y
		{
			get { return BaseData[1]; }
			set { BaseData[1] = value; }
		}
	}

	internal class HuffmanNodeQ : BaseHuffmanNode<HuffmanNodeQ>
	{
		public HuffmanNodeQ()
			: base(4) {}

		public int V
		{
			get { return BaseData[0]; }
			set { BaseData[0] = value; }
		}

		public int W
		{
			get { return BaseData[1]; }
			set { BaseData[1] = value; }
		}

		public int X
		{
			get { return BaseData[2]; }
			set { BaseData[2] = value; }
		}

		public int Y
		{
			get { return BaseData[3]; }
			set { BaseData[3] = value; }
		}
	}

	internal class HuffmanCode
	{
		public string Hcode;
		public int[] Data;

		public HuffmanCode(string code, int[] data)
		{
			Hcode = code;
			Data = data;
		}
	}

	internal class HuffmanTable
	{
		public readonly List<HuffmanCode> Codes = new List<HuffmanCode>();
	}
}