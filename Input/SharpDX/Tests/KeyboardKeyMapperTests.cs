using NUnit.Framework;

namespace DeltaEngine.Input.SharpDX.Tests
{
	public class KeyboardKeyMapperTests
	{
		[Test]
		public void Translate()
		{
			Assert.AreEqual(Key.O, SharpDX.KeyboardKeyMapper.Translate(
				global::SharpDX.DirectInput.Key.O));
			Assert.AreEqual(Key.CloseBrackets, SharpDX.KeyboardKeyMapper.Translate(
				global::SharpDX.DirectInput.Key.RightBracket));
		}
	}
}
