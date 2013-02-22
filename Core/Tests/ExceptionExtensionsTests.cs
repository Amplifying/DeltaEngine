using System.Diagnostics;
using NUnit.Framework;
using System;
using System.Reflection;

namespace DeltaEngine.Core.Tests
{
	public class ExceptionExtensionsTests
	{
		[Test]
		public void CheckForFatalException()
		{
			Assert.IsFalse(new Exception().IsFatal());
			Assert.IsTrue(new Exception("", new InvalidOperationException()).IsFatal());
			Assert.IsTrue(new InvalidOperationException().IsFatal());
			Assert.IsTrue(new OutOfMemoryException().IsFatal());
			Assert.IsTrue(new AccessViolationException().IsFatal());
			Assert.IsTrue(new StackOverflowException().IsFatal());
			Assert.IsTrue(new TargetInvocationException(null).IsFatal());
			Assert.IsFalse(new NullReferenceException(null).IsFatal());
		}

		[Test]
		public void CheckForWeakException()
		{
			Assert.IsFalse(new Exception().IsWeak());
			Assert.IsTrue(new ArgumentNullException().IsWeak());
		}

		[Test]
		public void MakeSureFatalExceptionsAreRethrown()
		{
			try
			{
				throw new InvalidOperationException();
			}
			catch (Exception ex)
			{
				Assert.IsTrue(ex.IsFatal());
			}
		}

		[Test]
		public void RethrowWeakExceptionIfNoDebuggerIsAttached()
		{
			try
			{
				throw new ArgumentNullException();
			}
			catch (Exception ex)
			{
				// This will fail if debugger is attached, but that is expected
				Assert.IsTrue(ex.IsWeak());
			}
		}

		[Test]
		public void CheckIfDebugModeAndNoDebuggerAttached()
		{
#if DEBUG
			Assert.AreEqual(!Debugger.IsAttached, ExceptionExtensions.IsDebugModeAndNoDebuggerAttached);
#endif
		}

		[Test]
		public void CheckIfReleaseMode()
		{
#if RELEASE
			Assert.IsTrue(ExceptionExtensions.IsReleaseMode);
#else
			Assert.IsFalse(ExceptionExtensions.IsReleaseMode);
#endif
		}
	}
}
