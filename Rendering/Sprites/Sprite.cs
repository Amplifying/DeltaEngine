using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// 2D sprite to be rendered, which is an image used as an Entity2D.
	/// </summary>
	public class Sprite : Entity2D
	{
		private Sprite()
			: base(Rectangle.Zero) {}

		public Sprite(string imageName, Rectangle drawArea)
			: this(ContentLoader.Load<Image>(imageName), drawArea) {}

		public Sprite(Image image, Rectangle drawArea)
			: base(drawArea)
		{
			Add(image);
			Add(image.BlendMode);
			Start<BatchRender>();
		}

		public Image Image
		{
			get { return Get<Image>(); }
			set
			{
				Set(value);
				Set(value.BlendMode);
			}
		}

		public BlendMode BlendMode
		{
			get { return Get<BlendMode>(); }
			set { Set(value); }
		}

		/// <summary>
		/// Responsible for rendering sprites in batches
		/// </summary>
		public class BatchRender : EventListener2D
		{
			public BatchRender(ScreenSpace screen, Drawing drawing)
			{
				this.screen = screen;
				this.drawing = drawing;
				spriteDrawQueue = new List<SpriteDrawContext>();
				verticesBatch = new List<VertexPositionColorTextured>();
				indicesBatch = new List<short>();
			}

			private readonly ScreenSpace screen;
			private readonly Drawing drawing;
			private readonly List<SpriteDrawContext> spriteDrawQueue;
			private readonly List<VertexPositionColorTextured> verticesBatch;
			private readonly List<short> indicesBatch;

			public override void ReceiveMessage(Entity2D entity, object message)
			{
				if (message is SortAndRender.AddToBatch)
					BatchSprite(entity);
			}

			private void BatchSprite(Entity2D entity)
			{
				SpriteDrawContext sprite;
				FillSpriteDrawContext(entity, out sprite);
				spriteDrawQueue.Add(sprite);
			}

			private void FillSpriteDrawContext(Entity2D entity, out SpriteDrawContext drawContext)
			{
				var rotation = entity.Rotation;
				var drawArea = entity.DrawArea;
				var color = entity.Color;
				var center = drawArea.Center;
				var rotationCenter = entity.Contains<RotationCenter>()
					? entity.Get<RotationCenter>().Value : center;

				drawContext.texture = entity.Get<Image>();
				drawContext.vertices = new[]
				{
					GetVertex(drawArea.TopLeft.RotateAround(rotationCenter, rotation), Point.Zero, color),
					GetVertex(drawArea.TopRight.RotateAround(rotationCenter, rotation), Point.UnitX, color),
					GetVertex(drawArea.BottomRight.RotateAround(rotationCenter, rotation), Point.One, color),
					GetVertex(drawArea.BottomLeft.RotateAround(rotationCenter, rotation), Point.UnitY, color)
				};
			}

			private struct SpriteDrawContext
			{
				public Image texture;
				public VertexPositionColorTextured[] vertices;
			}

			private VertexPositionColorTextured GetVertex(Point position, Point uv, Color color)
			{
				return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position), color, uv);
			}

			public override void ReceiveMessage(object message)
			{
				if (!(message is SortAndRender.RenderBatch) || spriteDrawQueue.Count == 0)
					return;

				RenderBatchesByTexture();
			}

			private void RenderBatchesByTexture()
			{
				Image batchTexture = spriteDrawQueue[0].texture;
				for (int pos = 0; pos < spriteDrawQueue.Count; ++pos)
					batchTexture = BatchSpriteByTexture(spriteDrawQueue[pos], batchTexture);

				RenderBatch(batchTexture);
				spriteDrawQueue.Clear();
			}

			private Image BatchSpriteByTexture(SpriteDrawContext sprite, Image batchTexture)
			{
				Image currentTexture = sprite.texture;
				if (currentTexture != batchTexture)
				{
					RenderBatch(currentTexture);
					batchTexture = currentTexture;
				}

				QueueSpriteRenderData(sprite.vertices);
				return batchTexture;
			}

			private void RenderBatch(Image texture)
			{
				drawing.EnableTexturing(texture);
				drawing.SetIndices(indicesBatch.ToArray(), indicesBatch.Count);
				drawing.DrawVerticesForSprite(VerticesMode.Triangles, verticesBatch.ToArray());
				verticesBatch.Clear();
				indicesBatch.Clear();
			}

			private void QueueSpriteRenderData(VertexPositionColorTextured[] vertices)
			{
				int verticesCount = verticesBatch.Count;
				indicesBatch.Add((short)(verticesCount + 0));
				indicesBatch.Add((short)(verticesCount + 1));
				indicesBatch.Add((short)(verticesCount + 2));
				indicesBatch.Add((short)(verticesCount + 0));
				indicesBatch.Add((short)(verticesCount + 2));
				indicesBatch.Add((short)(verticesCount + 3));
				verticesBatch.AddRange(vertices);
			}
		}
	}
}