using System;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// Base class and lowest available log level.
	/// Used for notifications like a successful operation or debug output.
	/// </summary>
	public class Info : IEquatable<Info>
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		protected Info() { }

		public Info(string text)
		{
			Text = text;
		}

		public string Text { get; protected set; }

		public override bool Equals(object other)
		{
			return other is Info && Equals((Info)other);
		}

		public bool Equals(Info other)
		{
			return other.Text == Text;
		}

		public override int GetHashCode()
		{
			return Text.GetHashCode();
		}
	}
}