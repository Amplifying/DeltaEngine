using NUnit.Framework;
using SharpDXKey = global::SharpDX.DirectInput.Key;

namespace DeltaEngine.Input.SharpDX.Tests
{
	public class KeyboardKeyMapperTests
	{
		[Test]
		public void Translate()
		{
			Assert.AreEqual(Key.O, SharpDX.KeyboardKeyMapper.Translate(SharpDXKey.O));
			Assert.AreEqual(Key.CloseBrackets, SharpDX.KeyboardKeyMapper.Translate(
				SharpDXKey.RightBracket));
		}
	}
}
