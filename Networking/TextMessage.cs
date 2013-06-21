using System;

namespace DeltaEngine.Networking
{
	public class TextMessage : IEquatable<TextMessage>
	{
		public TextMessage(string text)
		{
			Text = text;
		}

		public string Text { get; private set; }

		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		protected TextMessage() {}

		public override bool Equals(object other)
		{
			return other is TextMessage && Equals((TextMessage)other);
		}

		public bool Equals(TextMessage other)
		{
			return other.Text == Text;
		}

		public override int GetHashCode()
		{
			return Text.GetHashCode();
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Text + ")";
		}
	}
}