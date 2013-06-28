using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Fonts;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Holds a set of images and colors for Scenes UI controls, as well as the font to be used
	/// </summary>
	public struct Theme
	{
		public Appearance Button { get; set; }
		public Appearance ButtonMouseover { get; set; }
		public Appearance ButtonPressed { get; set; }
		public Font Font { get; set; }
		public Appearance Label { get; set; }
		public Appearance RadioButtonBackground { get; set; }
		public Appearance RadioButtonNotSelected { get; set; }
		public Appearance RadioButtonNotSelectedMouseover { get; set; }
		public Appearance RadioButtonSelected { get; set; }
		public Appearance RadioButtonSelectedMouseover { get; set; }
		public Appearance Slider { get; set; }
		public Appearance SliderPointer { get; set; }
		public Appearance SliderPointerMouseover { get; set; }
		public Appearance TextBox { get; set; }
		public Appearance TextBoxFocussed { get; set; }

		public struct Appearance
		{
			public Appearance(string imageName)
				: this(ContentLoader.Load<Image>(imageName)) { }

			public Appearance(Image image)
				: this(image, Color.White) {}

			public Appearance(string imageName, Color color)
				: this(ContentLoader.Load<Image>(imageName), color) { }

			public Appearance(Image image, Color color)
				: this()
			{
				Image = image;
				Color = color;
			}

			public Image Image { get; set; }
			public Color Color { get; set; }
		}
	}
}