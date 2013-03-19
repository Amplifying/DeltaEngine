using System;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Core;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// Base class and lowest available log level.
	/// Used for notifications like a successful operation or debug output.
	/// </summary>
	public class Info : BinaryData, IEquatable<Info>
	{
		public Info() { }

		public Info(string text)
		{
			Text = text;
			ProjectName = AssemblyExtensions.DetermineProjectName();
		}

		public string Text { get; protected set; }
		public string ProjectName { get; set; }

		public void Save(BinaryWriter writer)
		{
			writer.Write(Text);
			writer.Write(ProjectName);
		}

		public void Load(BinaryReader reader)
		{
			Text = reader.ReadString();
			ProjectName = reader.ReadString();
		}

		public override bool Equals(object other)
		{
			return other is Info && Equals((Info)other);
		}

		public bool Equals(Info other)
		{
			return other.Text == Text && other.ProjectName == ProjectName;
		}

		public override int GetHashCode()
		{
			return Text.GetHashCode() ^ ProjectName.GetHashCode();
		}
	}
}