using System;
using System.IO;

namespace DeltaEngine.Editor.Core
{
	public class CodeUnpacker
	{
		public CodeUnpacker(byte[] packedCodeData)
		{
			if (packedCodeData == null || packedCodeData.Length == 0)
				throw new NoPackedDataSpecified();
			packedData = packedCodeData;
		}

		public class NoPackedDataSpecified : Exception {}

		private readonly byte[] packedData;

		public void SaveToDirectory(string targetDirectory)
		{
			outputDirectory = targetDirectory;
			using (var dataStream = new MemoryStream(packedData))
			using (var streamReader = new BinaryReader(dataStream))
				SavePackedDataToOutputDirectory(streamReader);
		}

		private string outputDirectory;

		private void SavePackedDataToOutputDirectory(BinaryReader streamReader)
		{
			int fileCount = streamReader.ReadInt32();
			for (int i = 0; i < fileCount; i++)
				CreateFile(streamReader);
		}

		private void CreateFile(BinaryReader streamReader)
		{
			string relativeFilePath = streamReader.ReadString();
			int fileLength = streamReader.ReadInt32();
			byte[] fileData = streamReader.ReadBytes(fileLength);
			SaveDataToFile(relativeFilePath, fileData);
		}

		private void SaveDataToFile(string filePath, byte[] fileData)
		{
			string finalFilePath = Path.Combine(outputDirectory, filePath);
			string fileDirectory = Path.GetDirectoryName(finalFilePath);
			if (!Directory.Exists(fileDirectory))
				Directory.CreateDirectory(fileDirectory);
			File.WriteAllBytes(finalFilePath, fileData);
		}
	}
}
