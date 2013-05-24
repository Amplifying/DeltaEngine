using System;
using System.Diagnostics;
using System.Threading;

namespace DeltaEngine.Editor.Builder
{
	public class ProcessRunner
	{
		private const int NoTimeout = -1;

		public ProcessRunner(string filePath, string argumentsLine = "", int timeoutInMs = NoTimeout)
		{
			FilePath = filePath;
			this.argumentsLine = argumentsLine;
			WorkingDirectory = Environment.CurrentDirectory;
			this.timeoutInMs = timeoutInMs;
			Output = "";
			Errors = "";
		}

		protected string FilePath;
		protected string argumentsLine;
		protected int timeoutInMs;

		public string WorkingDirectory { get; set; }
		public string Output { get; private set; }
		public string Errors { get; private set; }

		public void Start()
		{
			using (nativeProcess = new Process())
			{
				SetupStartInfo();
				SetupProcessAndRun();
			}

			nativeProcess = null;
		}

		private Process nativeProcess;

		protected void SetupStartInfo()
		{
			nativeProcess.StartInfo.FileName = FilePath;
			nativeProcess.StartInfo.Arguments = argumentsLine;
			nativeProcess.StartInfo.WorkingDirectory = WorkingDirectory;
			nativeProcess.StartInfo.CreateNoWindow = true;
			nativeProcess.StartInfo.UseShellExecute = false;
			nativeProcess.StartInfo.RedirectStandardOutput = true;
			nativeProcess.StartInfo.RedirectStandardError = true;
		}

		private void SetupProcessAndRun()
		{
			// Helpful post how to avoid the possible deadlock of a process
			// http://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
			using (outputWaitHandle = new AutoResetEvent(false))
			using (errorWaitHandle = new AutoResetEvent(false))
				AttachToOutputStreamAndRunNativeProcess();

			errorWaitHandle = null;
			outputWaitHandle = null;
		}

		private AutoResetEvent outputWaitHandle;
		private AutoResetEvent errorWaitHandle;

		private void AttachToOutputStreamAndRunNativeProcess()
		{
			nativeProcess.OutputDataReceived += OnStandardOutputDataReceived;
			nativeProcess.ErrorDataReceived += OnErrorOutputDataReceived;
			StartNativeProcessAndWaitForExit();
		}

		private void OnStandardOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null)
			{
				outputWaitHandle.Set();
				return;
			}

			if (StandardOutputEvent != null)
				StandardOutputEvent(e.Data);

			Output += e.Data;
		}

		public event Action<string> StandardOutputEvent;

		private void OnErrorOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null)
			{
				errorWaitHandle.Set();
				return;
			}

			if (ErrorOutputEvent != null)
				ErrorOutputEvent(e.Data);

			Errors += e.Data;
		}

		public event Action<string> ErrorOutputEvent;

		private void StartNativeProcessAndWaitForExit()
		{
			nativeProcess.Start();
			nativeProcess.BeginOutputReadLine();
			nativeProcess.BeginErrorReadLine();
			WaitForExit();
			CheckExitCode();
		}

		private void WaitForExit()
		{
			if (!outputWaitHandle.WaitOne(timeoutInMs))
				throw new StandardOutputHasTimedOutException();

			if (!errorWaitHandle.WaitOne(timeoutInMs))
				throw new ErrorOutputHasTimedOutException();

			if (!nativeProcess.WaitForExit(timeoutInMs))
				throw new ProcessHasTimedOutException();
		}

		public class StandardOutputHasTimedOutException : Exception {}

		public class ErrorOutputHasTimedOutException : Exception {}

		public class ProcessHasTimedOutException : Exception {}

		private void CheckExitCode()
		{
			if (nativeProcess.ExitCode != 0)
				throw new ProcessTerminatedWithError();
		}

		public class ProcessTerminatedWithError : Exception {}
	}
}