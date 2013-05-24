using System;
using DeltaEngine.Core;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Builder
{
	public class BuildRequest : IEquatable<BuildRequest>
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction and derived classes.
		/// </summary>
		protected BuildRequest() {}

		public BuildRequest(string projectName, PlatformName platform, byte[] serializedProjectData)
		{
			ProjectName = projectName;
			SerializedProjectData = serializedProjectData;
			Platform = platform;
		}

		public string ProjectName { get; protected set; }
		public PlatformName Platform { get; protected set; }
		public byte[] SerializedProjectData { get; protected set; }

		public string SolutionFilePath { get; set; }

		public override bool Equals(object other)
		{
			return other is BuildRequest && Equals((BuildRequest)other);
		}

		public bool Equals(BuildRequest other)
		{
			return Platform == other.Platform &&
				ProjectName == other.ProjectName &&
				SerializedProjectData.Compare(other.SerializedProjectData);
		}

		public override int GetHashCode()
		{
			return SerializedProjectData.Length.GetHashCode();
		}
	}
}