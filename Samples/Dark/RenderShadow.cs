using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Dark
{
	public class RenderShadow : EventListener2D
	{
		public RenderShadow(ScreenSpace screen, Drawing drawing)
		{
			this.screen = screen;
			this.drawing = drawing;
			shadow = ContentLoader.Load<Image>("Shadow");
		}

		private readonly ScreenSpace screen;
		private readonly Drawing drawing;
		private readonly Image shadow;

		public void Handle(List<Entity> entities) {}

		public static Point LightPosition { get; set; }

		public override void ReceiveMessage(Entity2D entity, object message) 
		{
			//if (message is SortAndRenderEntity2D.TimeToRender)
				RenderShadowFromLight(entity, LightPosition);
		}

		private void RenderShadowFromLight(Entity entity, Point lightPosition)
		{
			var drawArea = entity.Get<Rectangle>();
			var center = drawArea.Center;
			var rotation = entity.Get<float>();
			var rotationCenter = entity.Contains<RotationCenter>() ? entity.Get<RotationCenter>().Value : center;
			if (lightPosition == center)
				return;

			var lightDirection = lightPosition.DirectionTo(center);
			lightDirection.Normalize();

			List<Edge> edges = new List<Edge>(6);
			edges.Add(new Edge(drawArea.TopLeft.RotateAround(rotationCenter, rotation), drawArea.TopRight.RotateAround(rotationCenter, rotation)));
			edges.Add(new Edge(drawArea.TopRight.RotateAround(rotationCenter, rotation), drawArea.BottomRight.RotateAround(rotationCenter, rotation)));
			edges.Add(new Edge(drawArea.BottomRight.RotateAround(rotationCenter, rotation), drawArea.BottomLeft.RotateAround(rotationCenter, rotation)));
			edges.Add(new Edge(drawArea.BottomLeft.RotateAround(rotationCenter, rotation), drawArea.TopLeft.RotateAround(rotationCenter, rotation)));

			List<Edge> castingEdges = new List<Edge>();
			foreach (var edge in edges)
			{
				float facing = edge.normal.DotProduct(lightDirection);
				if (facing < 0.0f)
					castingEdges.Add(edge);
			}

			var castingEdge = new Edge(Point.Zero, Point.Zero);
			if (castingEdges.Count > 1)
			{
				var edge1 = castingEdges[0];
				var edge2 = castingEdges[castingEdges.Count - 1];

				float edge1FirstDistance = lightPosition.DistanceToSquared(edge1.first);
				float edge1SecondDistance = lightPosition.DistanceToSquared(edge1.second);
				float edge2FirstDistance = lightPosition.DistanceToSquared(edge2.first);
				float edge2SecondDistance = lightPosition.DistanceToSquared(edge2.second);

				castingEdge = new Edge(edge1FirstDistance > edge1SecondDistance ? edge1.first : edge1.second,
						edge2FirstDistance > edge2SecondDistance ? edge2.first : edge2.second);
			}
			else
			{
				castingEdge = castingEdges[0];
			}

			ScaleEdge(ref castingEdge, entity);
			
			var bottomLeft = castingEdge.first;
			var bottomRight = castingEdge.second;

			var leftDirection = lightPosition.DirectionTo(bottomLeft);
			leftDirection.Normalize();
			var topLeft = bottomLeft + leftDirection;

			var rightDirection = lightPosition.DirectionTo(bottomRight);
			rightDirection.Normalize();
			var topRight = bottomRight + rightDirection;

			var vertices = new[]
				{
					GetVertex(topLeft, Point.Zero),
					GetVertex(topRight, Point.UnitX),
					GetVertex(bottomRight, Point.One),
					GetVertex(bottomLeft, Point.UnitY)
				};


			//drawing.DrawQuad(shadow, vertices);
		}

		private void ScaleEdge(ref Edge edge, Entity entity)
		{
			var direction = edge.first.DirectionTo(edge.second);
			direction.Normalize();

			var shrinkValue = entity.Contains<EdgeShrinkFactor>()
				? entity.Get<EdgeShrinkFactor>().Value : 0.0f;

			edge.first = edge.first + direction * shrinkValue;
			edge.second = edge.second - direction * shrinkValue;
		}

		private VertexPositionColorTextured GetVertex(Point position, Point uv)
		{
			return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position),
				new Color(0, 0, 0, 80), uv);
		}

		struct Edge
		{
			public Edge(Point first, Point second)
			{
				this.first = first;
				this.second = second;
				normal = new Point(second.Y - first.Y, first.X - second.X);
				normal.Normalize();
			}

			public Point first;
			public Point second;
			public Point normal;
		}
	}
}
