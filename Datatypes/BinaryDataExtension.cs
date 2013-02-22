using System;
using System.IO;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Helper for sending and receiving BinaryData objects
	/// </summary>
	public static class BinaryDataExtension
	{
		public static byte[] ToArrayWithLengthHeader(this BinaryData message)
		{
			byte[] data = ToArray(message);
			byte[] head = BitConverter.GetBytes(data.Length);
			var total = new byte[data.Length + head.Length];
			head.CopyTo(total, 0);
			data.CopyTo(total, head.Length);				
			return total;
		}

		public static byte[] ToArray(this BinaryData message)
		{
			using (var messageStream = new MemoryStream())
			using (var messageWriter = new BinaryWriter(messageStream))
			{
				new BinaryDataFactory().Save(message, messageWriter);
				return messageStream.ToArray();
			}
		}

		public static BinaryData ToBinaryData(this byte[] data)
		{
			using (var messageStream = new MemoryStream(data))
			using (var messageReader = new BinaryReader(messageStream))
				return new BinaryDataFactory().Load(messageReader);
		}
	}
}