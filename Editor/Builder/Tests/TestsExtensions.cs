using System;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Builder.Tests
{
	internal static class TestsExtensions
	{
		public static T CloneViaBinaryData<T>(this T dataObjectToClone)
		{
			using (var dataStream = new MemoryStream())
			{
				var writer = new BinaryWriter(dataStream);
				dataObjectToClone.Save(writer);
				dataStream.Seek(0, SeekOrigin.Begin);
				var reader = new BinaryReader(dataStream);
				var clonedObject = (T)reader.Create();
				return clonedObject;
			}
		}

		public static AppPackage GetTestPackageForWP7(this PlatformName platform)
		{
			if (platform != PlatformName.WindowsPhone7)
				throw new NotSupportedPlatform(platform);

			return new AppPackage(platform)
			{
				PackageFileName = "TestPackage.dummy",
				PackageFileData = new byte[0],
				PackageGuid = Guid.Empty,
			};
		}

		public class NotSupportedPlatform : Exception
		{
			public NotSupportedPlatform(PlatformName platform) : base(platform.ToString()) {}
		}
	}
}