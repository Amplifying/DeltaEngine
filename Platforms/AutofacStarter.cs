using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Autofac.Core;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Logging;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Starts an application on demand by registering, resolving and running it
	/// </summary>
	public class AutofacStarter : AutofacResolver
	{
		public AutofacStarter()
		{
			if (StackTraceExtensions.IsUnitTest() && !StackTraceExtensions.StartedFromNCrunch)
				CheckApprovalImageAfterFirstFrame();
		}
		
		internal void CreateEntitySystemAndAddAsRunner()
		{
			var entitySystem = new EntitySystem(new AutofacHandlerResolver(this));
			RegisterInstanceAsRunnerOrPresenterIfPossible(entitySystem);
			EntitySystem.Use(entitySystem);
		}

		private class AutofacHandlerResolver : HandlerResolver
		{
			public AutofacHandlerResolver(Resolver resolver)
			{
				this.resolver = resolver;
			}

			private readonly Resolver resolver;

			public Handler Resolve(Type handlerType)
			{
				return resolver.Resolve(handlerType) as Handler;
			}
		}

		private void Initialize<AppEntryRunner>(int instancesToCreate)
		{
			Initialized += () =>
			{
				for (int num = 0; num < instancesToCreate; num++)
					Resolve<AppEntryRunner>();
			};
		}

		protected event Action Initialized;

		public void CheckApprovalImageAfterFirstFrame()
		{
			var approvalTestName = StackTraceExtensions.GetApprovalTestName();
			if (String.IsNullOrEmpty(approvalTestName))
				return;

			firstFrameApprovalImageFilename = "..\\..\\" + approvalTestName + "." +
				GetType().Name.Replace("Resolver", "") + ".png";
		}

		private string firstFrameApprovalImageFilename;

		public virtual void Run(Action optionalRunCode)
		{
			RaiseInitializedEvent();

			var window = Resolve<Window>();
			do
				TryRunAllRunnersAndPresenters(optionalRunCode);
			while (!window.IsClosing);
		}

		protected void RaiseInitializedEvent()
		{
			try
			{
				if (Initialized != null)
					Initialized();

				Initialized = null;
			}
			catch (DependencyResolutionException exception)
			{
				Logger.Current.Error(exception.InnerException ?? exception);
				if (Debugger.IsAttached || StackTraceExtensions.StartedFromNCrunch)
					throw;

				DisplayMessageBoxAndCloseApp(exception.InnerException ?? exception,
					"Fatal Initialization Error");
			}
#if !DEBUG 
	// In debug mode we want to crash where the actual exception happens, not here.
			catch (Exception exception)
			{
				Logger.Current.Error(exception);
				if (Debugger.IsAttached || StackTraceExtensions.StartedFromNCrunch)
					throw;

				DisplayMessageBoxAndCloseApp(exception, "Fatal Initialization Error");
			}
#endif
		}

		//TODO remove
		internal void TryRunAllRunnersAndPresenters(Action optionalRunCode)
		{
#if !DEBUG
			try
			{
				RunAllRunners();
				if (optionalRunCode != null)
					optionalRunCode();

				RunAllPresenters();
			}
			// In debug mode we want to crash where the actual exception happens, not here.
			catch (Exception exception)
			{
				Logger.Current.Error(exception);
				if (Debugger.IsAttached || StackTraceExtensions.StartedFromNCrunch)
					throw;
				if (exception.IsWeak())
					return; //ncrunch: no coverage

				DisplayMessageBoxAndCloseApp(exception, "Fatal Runtime Error");
			}
#else
			RunAllRunners();
			if (optionalRunCode != null)
				optionalRunCode();

			RunAllPresenters();
			MakeApprovalImageScreenshotAfterFirstFrame();
#endif
		}

		private bool HasApprovalImage
		{
			get { return !String.IsNullOrEmpty(firstFrameApprovalImageFilename); }
		}

		internal void MakeApprovalImageScreenshotAfterFirstFrame()
		{
			if (madeApprovalImage || !HasApprovalImage)
				return;

			madeApprovalImage = true;
			Resolve<ScreenshotCapturer>().MakeScreenshot(firstFrameApprovalImageFilename);
		}

		private bool madeApprovalImage;

		public override void Dispose()
		{
			if (madeApprovalImage)
				CheckApprovalTestResult();
			base.Dispose();
		}

		protected void CheckApprovalTestResult()
		{
			if (!File.Exists(firstFrameApprovalImageFilename))
				throw new FileNotFoundException(
					"Unable to approve test as no image was generated and saved after the first frame.",
					firstFrameApprovalImageFilename);

			approvedImageFileName =
				firstFrameApprovalImageFilename.Replace("." + GetType().Name.Replace("Resolver", ""),
					".approved");
			if (File.Exists(approvedImageFileName))
				CompareImages();
			else
				UseFirstFrameImageAsApprovedImage();
		}

		private string approvedImageFileName;

		private void CompareImages()
		{
			float difference = 0.0f;
			using (var approvedBitmap = new Bitmap(approvedImageFileName))
			using (var compareBitmap = new Bitmap(firstFrameApprovalImageFilename))
				if (approvedBitmap.Width != compareBitmap.Width ||
					approvedBitmap.Height != compareBitmap.Height)
					ImagesAreDifferent("Approved image size " + approvedBitmap.Width + "x" +
						approvedBitmap.Height + " is different from the compare image size: " +
						compareBitmap.Width + "x" + compareBitmap.Height);
				else
					difference = CompareImageContent(approvedBitmap, compareBitmap);

			if (difference < 0.05f)
				ApprovalTestSucceeded(difference);
			else
				ImagesAreDifferent("Difference " + difference.ToString("0.00") + "%");
		}

		private void ImagesAreDifferent(string errorMessage)
		{
			errorMessage = "Approval test failed with " + GetType().Name + ", resulting image is " +
				"different from the approved one:\n" + Path.GetFileName(firstFrameApprovalImageFilename) +
				"\n" + errorMessage;
			LaunchTortoiseIDiffIfAvailable();
			throw new ApprovalTestFailed(errorMessage);
		}

		private void LaunchTortoiseIDiffIfAvailable()
		{
			if (File.Exists(TortoiseIDiffFilePath))
				Process.Start(TortoiseIDiffFilePath,
					"/left:\"" + approvedImageFileName + "\" /right:\"" + firstFrameApprovalImageFilename +
						"\" /showinfo");
			else
			{
				Process.Start(approvedImageFileName);
				Process.Start(firstFrameApprovalImageFilename);
			}
		}

		private const string TortoiseIDiffFilePath =
			@"C:\Program Files\TortoiseSVN\bin\TortoiseIDiff.exe";

		private class ApprovalTestFailed : Exception
		{
			public ApprovalTestFailed(string errorMessage) : base(errorMessage) { }
		}

		private static unsafe float CompareImageContent(Bitmap approvedBitmap, Bitmap compareBitmap)
		{
			var approvedData = GetBitmapData(approvedBitmap);
			var compareData = GetBitmapData(compareBitmap);
			float difference = GetImageDifference(approvedBitmap.Width, approvedBitmap.Height,
				(byte*)approvedData.Scan0.ToPointer(), (byte*)compareData.Scan0.ToPointer());
			approvedBitmap.UnlockBits(approvedData);
			compareBitmap.UnlockBits(compareData);
			return difference;
		}

		private void ApprovalTestSucceeded(float difference)
		{
			Console.WriteLine("Approval test succeeded, difference to approved image: " +
				difference.ToString("0.00") + "%\nDeleting " +
				Path.GetFileName(firstFrameApprovalImageFilename));
			File.Delete(firstFrameApprovalImageFilename);
		}

		private static BitmapData GetBitmapData(Bitmap bitmap)
		{
			return bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
		}

		private static unsafe float GetImageDifference(int width, int height, byte* approvedBytes,
			byte* compareBytes)
		{
			float totalDifference = 0.0f;
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
				{
					int index = (y * width + x) * 3;
					int blue = compareBytes[index] - approvedBytes[index];
					int green = compareBytes[index + 1] - approvedBytes[index + 1];
					int red = compareBytes[index + 2] - approvedBytes[index + 2];
					int difference = Math.Abs(blue * 12 + green * 58 + red * 30);
					totalDifference += difference / 255.0f;
				}
			return totalDifference / (width * height);
		}

		private void UseFirstFrameImageAsApprovedImage()
		{
			File.Move(firstFrameApprovalImageFilename, approvedImageFileName);
			Console.WriteLine(Path.GetFileName(approvedImageFileName) +
				" did not exist and was created from this test. Run again to compare the result.");
		}

		private void DisplayMessageBoxAndCloseApp(Exception exception, string title)
		{
			var window = Resolve<Window>();
			if (window.ShowMessageBox(title, "Unable to continue: " + exception,
				MessageBoxButton.Ignore) != MessageBoxButton.Ignore)
				window.Dispose();
		}

		public void AdvanceTimeAndExecuteRunners(float timeToAddInSeconds = 1/60.0f)
		{
			var simulateRunTicks = Math.Max(1, (int)Math.Round(timeToAddInSeconds * 60));
			for (int tick = 0; tick < simulateRunTicks; tick++)
			{
				RunAllRunners();
				RunAllPresenters();
			}
		}
	}
}