using NUnit.Framework;
using SharpDXKey = SharpDX.DirectInput.Key;

namespace DeltaEngine.Input.SharpDX.Tests
{
	public class KeyboardKeyMapperTests
	{
		[Test]
		public void Translate()
		{
			Assert.AreEqual(Key.O, KeyboardKeyMapper.Translate(SharpDXKey.O));
			Assert.AreEqual(Key.CloseBrackets, KeyboardKeyMapper.Translate(SharpDXKey.RightBracket));
		}
	}
}