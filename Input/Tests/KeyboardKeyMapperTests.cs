using DeltaEngine.Input.SharpDX;
using NUnit.Framework;
using DInput = SharpDX.DirectInput;

namespace DeltaEngine.Input.Tests
{
	public class KeyboardKeyMapperTests
	{
		[Test]
		public void Translate()
		{
			var mapper = new KeyboardKeyMapper();
			Assert.AreEqual(Key.O, mapper.Translate(DInput.Key.O));
			Assert.AreEqual(Key.CloseBrackets, mapper.Translate(DInput.Key.RightBracket));
		}
	}
}
