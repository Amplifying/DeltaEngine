using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Editor.Builder
{
	public class CodeData
	{
		public CodeData(byte[] dataInBytes)
		{
			this.dataInBytes = dataInBytes;
		}

		private byte[] dataInBytes;

		public CodeData(string sourceDirectory)
		{
			codeSourceDirectory = sourceDirectory;
			LoadDataFromSourceDirectory();
		}

		private readonly string codeSourceDirectory;

		private void LoadDataFromSourceDirectory()
		{
			if (!Directory.Exists(codeSourceDirectory))
				throw new DirectoryDoesNotExist(codeSourceDirectory);

			using (var dataStream = new MemoryStream())
			using (var streamWriter = new BinaryWriter(dataStream))
			{
				SaveRelevantFilesToStream(streamWriter);
				dataInBytes = dataStream.ToArray();
			}
		}

		public class DirectoryDoesNotExist : Exception
		{
			public DirectoryDoesNotExist(string directory) : base(directory) { }
		}

		private void SaveRelevantFilesToStream(BinaryWriter streamWriter)
		{
			string[] filesToPack = GetRelevantFiles();
			streamWriter.Write(filesToPack.Length);
			foreach (string filePath in filesToPack)
				WriteFileToStream(streamWriter, filePath);
		}

		private string[] GetRelevantFiles()
		{
			var filePathList = new List<string>();
			foreach (string filePath in GetAllFiles(codeSourceDirectory))
				if (IsNecessaryFile(filePath))
					filePathList.Add(filePath);

			return filePathList.ToArray();
		}

		private static IEnumerable<string> GetAllFiles(string directory)
		{
			return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
		}

		private static bool IsNecessaryFile(string filePath)
		{
			return !IsCompilerOutputFile(filePath) && !IsNugetFileOrPackage(filePath) &&
				!IsNoNecessaryCodeFile(filePath);
		}

		private static bool IsCompilerOutputFile(string filePath)
		{
			return filePath.Contains(@"\bin\") || filePath.Contains(@"\obj\");
		}

		private static bool IsNugetFileOrPackage(string filePath)
		{
			return Directory.Exists(".nuget") || filePath.Contains(@"\packages\");
		}

		private static bool IsNoNecessaryCodeFile(string filePath)
		{
			return filePath.EndsWith(".user") || filePath.EndsWith(".suo") ||
				filePath.EndsWith(".cachefile") || filePath.EndsWith(".pch") ||
				filePath.Contains(".ncrunch");
		}

		private void WriteFileToStream(BinaryWriter streamWriter, string filePath)
		{
			string relativeFilePath = filePath.Substring(codeSourceDirectory.Length + 1);
			streamWriter.Write(relativeFilePath);

			byte[] fileBytes = File.ReadAllBytes(filePath);
			streamWriter.Write(fileBytes.Length);
			streamWriter.Write(fileBytes);
		}

		public byte[] GetBytes()
		{
			return dataInBytes;
		}

		public void SaveToDirectory(string targetDirectory)
		{
			codeTargetDirectory = targetDirectory;
			using (var dataStream = new MemoryStream(dataInBytes))
			using (var streamReader = new BinaryReader(dataStream))
			{
				int fileCount = streamReader.ReadInt32();
				for (int i = 0; i < fileCount; i++)
					CreateFile(streamReader);
			}
		}

		private string codeTargetDirectory;

		private void CreateFile(BinaryReader streamReader)
		{
			string relativeFilePath = streamReader.ReadString();
			int fileLength = streamReader.ReadInt32();
			byte[] fileData = streamReader.ReadBytes(fileLength);
			WriteDataToFile(relativeFilePath, fileData);
		}

		private void WriteDataToFile(string filePath, byte[] fileData)
		{
			string finalFilePath = Path.Combine(codeTargetDirectory, filePath);
			string fileDirectory = Path.GetDirectoryName(finalFilePath);
			if (!Directory.Exists(fileDirectory))
				Directory.CreateDirectory(fileDirectory);

			File.WriteAllBytes(finalFilePath, fileData);
		}
	}
}
