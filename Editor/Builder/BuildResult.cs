using System;

namespace DeltaEngine.Editor.Builder
{
	public class BuildResult : AppPackage, IEquatable<BuildResult>
	{
		public BuildResult() {}

		public BuildResult(AppPackage appPackage)
		{
			AppName = appPackage.AppName;
			Platform = appPackage.Platform;
			PackageFileName = appPackage.PackageFileName;
			PackageGuid = appPackage.PackageGuid;
			PackageFileData = appPackage.PackageFileData;
		}

		public string BuildError { get; set; }

		public bool Equals(BuildResult other)
		{
			return BuildError == other.BuildError && Equals((AppPackage)other);
		}

		public override bool Equals(object other)
		{
			return other is BuildResult && Equals((BuildResult)other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
