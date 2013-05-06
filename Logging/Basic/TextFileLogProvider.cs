using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using DeltaEngine.Core;

namespace DeltaEngine.Logging.Basic
{
	/// <summary>
	/// Writes into a log text file and opens it in a editor after execution.
	/// </summary>
	public class TextFileLogProvider : LogProvider, IDisposable
	{
		public TextFileLogProvider()
		{
			var logFolder = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Delta Engine");
			if (!Directory.Exists(logFolder))
				Directory.CreateDirectory(logFolder);
			filePath = Path.Combine(logFolder, AssemblyExtensions.DetermineProjectName() + FileExtension);
		}

		private readonly string filePath;
		private const string FileExtension = ".txt";

		private StreamWriter CreateLogFileIfNotDoneYet()
		{
			if (writer != null)
				return writer;

			var logFile = File.Create(filePath);
			writer = new StreamWriter(logFile);
			writer.AutoFlush = true;
			writer.WriteLine("Delta Engine Log");
			return writer;
		}

		private StreamWriter writer;

		public void Log(Info info)
		{
			CreateLogFileIfNotDoneYet().WriteLine(TimeStamp + info.Text);
		}

		protected string TimeStamp
		{
			get { return DateTime.Now.ToString("T") + " "; }
		}

		public void Log(Warning warning)
		{
			CreateLogFileIfNotDoneYet().WriteLine(TimeStamp + "Warning: " + warning.Text);
		}

		public void Log(Error error)
		{
			CreateLogFileIfNotDoneYet().WriteLine(TimeStamp + "Error: " + error.Text);
		}
		
		public void Dispose()
		{
			if (writer != null)
				writer.Close();
			OpenLogFileInEditor();
		}

		private void OpenLogFileInEditor()
		{
			if (!ExceptionExtensions.IsDebugMode || writer == null)
				return;

			try
			{
				Process.Start(filePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to open engine log file in text editor: " + ex.Message);
			}
		}
	}
}