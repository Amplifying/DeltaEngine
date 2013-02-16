using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Devices;
using DeltaEngine.Rendering;

namespace LogoApp.Tests
{
	/// <summary>
	/// Extends BouncingLogo even more by adding gravity into the mix (plus friction).
	/// </summary>
	public class BouncingLogoWithGravity : BouncingLogoWithFriction
	{
		public BouncingLogoWithGravity(Content content, Randomizer random, InputCommands inputCommands)
			: base(content, random)
		{
			inputCommands.Add(Key.Space, () => Reset(random));
			inputCommands.Add(MouseButton.Left, mouse => Reset(random));
			inputCommands.Add((Touch touch) => Reset(random));
		}

		private void Reset(Randomizer random)
		{
			DrawArea = Rectangle.FromCenter(Point.Half, new Size(random.Get(0.1f, 0.2f)));
			velocity = new Point(random.Get(-0.4f, +0.4f), random.Get(-0.4f, +0.4f));
		}

		protected override void Render(Renderer renderer, Time time)
		{
			var gravity = new Point(0.0f, 9.81f);
			if (DrawArea.Bottom < renderer.Screen.Bottom)
				velocity += gravity * 0.2f * time.CurrentDelta;
			base.Render(renderer, time);
		}
	}
}