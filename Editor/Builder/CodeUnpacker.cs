using System;
using System.IO;

namespace DeltaEngine.Editor.Builder
{
	public class CodeUnpacker
	{
		public CodeUnpacker(byte[] packedCodeData)
		{
			packedData = packedCodeData;
		}

		private readonly byte[] packedData;

		public void SaveToDirectory(string targetDirectory)
		{
			outputDirectory = targetDirectory;
			if (Directory.Exists(outputDirectory))
				throw new TargetDirectoryExistsAlready(outputDirectory);

			SavePackedDataToOutputDirectory();
		}

		public class TargetDirectoryExistsAlready : Exception
		{
			public TargetDirectoryExistsAlready(string targetDirectory) : base(targetDirectory) {}
		}

		private string outputDirectory;

		private void SavePackedDataToOutputDirectory()
		{
			Directory.CreateDirectory(outputDirectory);
			using (var dataStream = new MemoryStream(packedData))
			using (var streamReader = new BinaryReader(dataStream))
			{
				int fileCount = streamReader.ReadInt32();
				for (int i = 0; i < fileCount; i++)
					CreateFile(streamReader);
			}
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
