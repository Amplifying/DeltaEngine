using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Graphics;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Any visual or integration test with the ApproveFirstFrameScreenshot attribute will create
	/// a screenshot and then be compared with the previous result via test Start methods.
	/// </summary>
	public abstract class TestApprovalImages
	{
		protected bool IgnoreSlowTestIfStartedViaNCrunchOrNunitConsole(Type resolverType)
		{
			StackTraceExtensions.IsVisualTestCase = false;
			var stackFrames = new StackTrace().GetFrames();
			if (stackFrames != null)
				foreach (var frame in stackFrames)
					IsFrameInVisualTestCase(frame);

			if (resolverType == typeof(MockResolver) || NCrunchAllowIntegrationTests)
				return false;

			return IsStartedFromNunit() || StackTraceExtensions.StartedFromNCrunch;
		}

		//ncrunch: no coverage start
		protected virtual bool NCrunchAllowIntegrationTests
		{
			get { return false; }
		}

		private void IsFrameInVisualTestCase(StackFrame frame)
		{
			object[] attributes = frame.GetMethod().GetCustomAttributes(false);
			foreach (object attribute in attributes)
			{
				var testCase = attribute as TestCaseAttribute;
				//// ReSharper disable RedundantLogicalConditionalExpressionOperand
				if (testCase != null && !ForceAllVisualTestsToBehaveLikeIntegrationTests &&
					(testCase.Category == "Visual" || testCase.Ignore))
					StackTraceExtensions.IsVisualTestCase = true; //ncrunch: no coverage
			}
		}

		protected virtual bool ForceAllVisualTestsToBehaveLikeIntegrationTests
		{
			get { return false; }
		}
		
		private static bool IsStartedFromNunit()
		{
			string currentDomainName = AppDomain.CurrentDomain.FriendlyName;
			return currentDomainName == "NUnit Domain" || currentDomainName.StartsWith("test-domain-");
		}

		protected Action AddApprovalCheckToRunCode(Resolver resolver, Action runCode)
		{
			var frames = new StackTrace().GetFrames();
			isApprovalTest = !StackTraceExtensions.StartedFromNCrunch &&
				resolver.GetType() != typeof(AutofacStarterForMockResolver) && frames.IsApprovalTest();
			if (!isApprovalTest)
				return runCode;

			resolverName = resolver.GetType().Name;
			firstFrameImageFilename = "..\\..\\" + frames.GetClassName() + "." +
				frames.GetTestMethodName() + "." + resolverName.Replace("Resolver", "") + ".png";
			bool tookScreenshot = false;
			return () =>
			{
				if (runCode != null)
					runCode();

				if (tookScreenshot)
					return;
				tookScreenshot = true;
				resolver.Resolve<ScreenshotCapturer>().MakeScreenshot(firstFrameImageFilename);
			};
		}

		private bool isApprovalTest;
		private string resolverName;
		private string firstFrameImageFilename;

		protected void CheckApprovalTestResult()
		{
			if (!isApprovalTest)
				return;

			if (!File.Exists(firstFrameImageFilename))
				throw new FileNotFoundException(
					"Unable to approve test as no image was generated and saved after the first frame.",
					firstFrameImageFilename);

			approvedImageFileName =
				firstFrameImageFilename.Replace("." + resolverName.Replace("Resolver", ""), ".approved");
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
			using (var compareBitmap = new Bitmap(firstFrameImageFilename))
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
			errorMessage = "Approval test failed with " + resolverName + ", resulting image is " +
				"different from the approved one:\n" + Path.GetFileName(firstFrameImageFilename) + "\n" +
				errorMessage;
			LaunchTortoiseIDiffIfAvailable();
			throw new ApprovalTestFailed(errorMessage);
		}

		private void LaunchTortoiseIDiffIfAvailable()
		{
			if (File.Exists(TortoiseIDiffFilePath))
				Process.Start(TortoiseIDiffFilePath,
					"/left:\"" + approvedImageFileName + "\" /right:\"" + firstFrameImageFilename +
					"\" /showinfo");
			else
				Process.Start(firstFrameImageFilename);
		}

		private const string TortoiseIDiffFilePath =
			@"C:\Program Files\TortoiseSVN\bin\TortoiseIDiff.exe";

		private class ApprovalTestFailed : Exception
		{
			public ApprovalTestFailed(string errorMessage) : base(errorMessage) {}
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
				difference.ToString("0.00") + "%\nDeleting " + Path.GetFileName(firstFrameImageFilename));
			File.Delete(firstFrameImageFilename);
		}

		private static BitmapData GetBitmapData(Bitmap bitmap)
		{
			return bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
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
			File.Move(firstFrameImageFilename, approvedImageFileName);
			Console.WriteLine(Path.GetFileName(approvedImageFileName) +
				" did not exist and was created from this test. Run again to compare the result.");
		}
	}
}