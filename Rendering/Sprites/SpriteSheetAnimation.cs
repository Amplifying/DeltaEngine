using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Sprites
{
	public class SpriteSheetAnimation : Sprite
	{
		public SpriteSheetAnimation(string contentName, Rectangle initialDrawArea)
			: this(ContentLoader.Load<SpriteSheetData>(contentName), initialDrawArea) {}

		private SpriteSheetAnimation(SpriteSheetData data, Rectangle drawArea)
			: base(data.Image, drawArea)
		{
			Stop<BatchRender>();
			Add(data);
			Add(0);
			Start<Update, SpriteSheetRender>();
			IsPlaying = true;
			Elapsed = 0.0f;
			CurrentFrame = 0;
		}

		public float Elapsed { get; set; }
		public int CurrentFrame { get; private set; }
		public bool IsPlaying { get; set; }

		public float Duration
		{
			get { return Get<SpriteSheetData>().Duration; }
			set { Get<SpriteSheetData>().Duration = value; }
		}

		internal void InvokeFinalFrame()
		{
			if (FinalFrame != null)
				FinalFrame();
		}

		public event Action FinalFrame;

		/// <summary>
		/// Updates current frame for a sprite animation
		/// </summary>
		public class Update : Behavior2D
		{
			public override void Handle(Entity2D entity)
			{
				var animation = entity as SpriteSheetAnimation;
				if (!animation.IsPlaying)
					return;
				var animationData = animation.Get<SpriteSheetData>();
				animation.Elapsed += Time.Current.Delta;
				animation.Elapsed = animation.Elapsed % animationData.Duration;
				animation.CurrentFrame =
					(int)(animationData.UVs.Count * animation.Elapsed / animationData.Duration);
				entity.Set(animation.CurrentFrame);
				if (animation.CurrentFrame == animationData.UVs.Count - 1)
					animation.InvokeFinalFrame();
			}

			public override Priority Priority
			{
				get { return Priority.First; }
			}
		}

		/// <summary>
		/// Responsible for rendering character sheet animations
		/// </summary>
		public class SpriteSheetRender : EventListener2D
		{
			public SpriteSheetRender(ScreenSpace screen, Drawing drawing)
			{
				this.screen = screen;
				this.drawing = drawing;
				drawingSprites = new Dictionary<Image, SpriteInfo>();
			}

			private readonly ScreenSpace screen;
			private readonly Drawing drawing;
			private readonly Dictionary<Image, SpriteInfo> drawingSprites;

			public override void ReceiveMessage(Entity2D entity, object message)
			{
				if (message is SortAndRender.AddToBatch)
					AddToBatch(entity);
			}

			private void AddToBatch(Entity2D entity)
			{
				var rotation = entity.Rotation;
				var drawArea = entity.DrawArea;
				var color = entity.Color;
				var center = drawArea.Center;
				var rotationCenter = entity.Contains<RotationCenter>()
					? entity.Get<RotationCenter>().Value : center;
				var uv = entity.Get<SpriteSheetData>().UVs[entity.Get<int>()];

				var newVertices = new List<VertexPositionColorTextured>
				{
					GetVertex(drawArea.TopLeft.RotateAround(rotationCenter, rotation), uv.TopLeft, color),
					GetVertex(drawArea.TopRight.RotateAround(rotationCenter, rotation), uv.TopRight, color),
					GetVertex(drawArea.BottomRight.RotateAround(rotationCenter, rotation), uv.BottomRight,
						color),
					GetVertex(drawArea.BottomLeft.RotateAround(rotationCenter, rotation), uv.BottomLeft, color)
				};
				var image = entity.Get<Image>();
				CheckIfNewImage(image, newVertices);
			}

			private void CheckIfNewImage(Image image, List<VertexPositionColorTextured> newVertices)
			{
				if (!drawingSprites.ContainsKey(image))
					drawingSprites.Add(image,
						new SpriteInfo { vertices = newVertices, indices = new List<short> { 0, 1, 2, 0, 2, 3 } });
				else
					AddNewVerticesToList(image, newVertices);
			}

			private void AddNewVerticesToList(Image image, List<VertexPositionColorTextured> newVertices)
			{
				SpriteInfo spriteInfo;
				drawingSprites.TryGetValue(image, out spriteInfo);
				SetIndicesOfNewVertices(spriteInfo);
				foreach (var vertice in newVertices)
					spriteInfo.vertices.Add(vertice);
			}

			private static void SetIndicesOfNewVertices(SpriteInfo spriteInfo)
			{
				spriteInfo.indices.Add((short)spriteInfo.vertices.Count);
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 1));
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 2));
				spriteInfo.indices.Add((short)spriteInfo.vertices.Count);
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 2));
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 3));
			}

			private VertexPositionColorTextured GetVertex(Point position, Point uv, Color color)
			{
				return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position), color, uv);
			}

			private struct SpriteInfo
			{
				public List<VertexPositionColorTextured> vertices;
				public List<short> indices;
			}

			public override void ReceiveMessage(object message)
			{
				if (!(message is SortAndRender.RenderBatch) || drawingSprites.Count == 0)
					return;

				foreach (var sprite in drawingSprites)
					drawing.DrawQuad(sprite.Key, sprite.Value.vertices, sprite.Value.indices);

				drawingSprites.Clear();
			}
		}
	}
}