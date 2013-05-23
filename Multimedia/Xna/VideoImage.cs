using System.IO;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Platforms;
using Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Multimedia.Xna
{
	[IgnoreForResolver]
	public class VideoImage : XnaImage
	{
		public VideoImage(VideoRenderingDependencies rendering, VideoPlayer player)
			: base(rendering.Drawing, rendering.GraphicsDevice, player.GetTexture())
		{
			this.player = player;
		}

		private readonly VideoPlayer player;

		protected override void LoadData(Stream fileData) {}

		public void UpdateTexture()
		{
			NativeTexture = player.GetTexture();
		}
	}
}