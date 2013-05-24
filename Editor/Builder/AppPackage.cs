using System;
using DeltaEngine.Core;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Builder
{
	public class AppPackage : IEquatable<AppPackage>
	{
		public AppPackage() {}

		public AppPackage(PlatformName platform)
		{
			Platform = platform;
		}

		public PlatformName Platform { get; set; }
		public string PackageFileName { get; set; }
		public string AppName { get; set; }
		public Guid PackageGuid { get; set; }
		public byte[] PackageFileData { get; set; }

		public bool Equals(AppPackage other)
		{
			return PackageFileName == other.PackageFileName && Platform == other.Platform &&
				PackageFileData.Compare(other.PackageFileData);
		}

		public override bool Equals(object other)
		{
			return other is AppPackage && Equals((AppPackage)other);
		}

		public override int GetHashCode()
		{
			return PackageFileName.GetHashCode();
		}

		public override string ToString()
		{
			return GetType().Name + "(" + AppName + ", " + Platform + ", " + PackageFileName + ")";
		}
	}
}
