using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// Spritesheet for an animation, 
	/// </summary>
	public class SpriteSheet
	{
		public SpriteSheet(string contentName, ContentLoader contentLoader)
		{
			Image = contentLoader.Load<Image>(contentName);
		}

		public Image Image { get; private set; }
		public int PixelWidthOfFrame { get; private set; }
		public int PixelHeightOfFrame { get; private set; }
		public int NumOfFramesHorizontal { get; private set; }
		public int NumOfFramesVertical { get; private set; } 
	}
}
