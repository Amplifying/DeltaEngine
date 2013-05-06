using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Physics2D.Farseer
{
	/// <summary>
	/// Implements the Farseer flavor of physics bodies
	/// </summary>
	internal class FarseerBody : PhysicsBody
	{
		public FarseerBody(Body body)
		{
			Body = body;
			Body.BodyType = BodyType.Dynamic;
		}

		public Body Body { get; private set; }
		internal UnitConverter UnitConverter { get; set; }

		public Point Position
		{
			get
			{
				var position = UnitConverter.Convert(UnitConverter.ToDisplayUnits(Body.Position));
				return position;
			}
			set
			{
				Vector2 position = UnitConverter.Convert(value);
				Body.Position = UnitConverter.ToSimUnits(position);
			}
		}

		public bool IsStatic
		{
			get { return Body.IsStatic; }
			set { Body.IsStatic = value; }
		}

		public float Restitution
		{
			get { return Body.Restitution; }
			set { Body.Restitution = value; }
		}

		public float Friction
		{
			get { return Body.Friction; }
			set { Body.Friction = value; }
		}

		public float Rotation
		{
			get { return Body.Rotation.RadiansToDegrees(); }
			set { Body.Rotation = value.DegreesToRadians(); }
		}

		public void ApplyLinearImpulse(Point impulse)
		{
			Vector2 fImpulse = UnitConverter.Convert(impulse);
			Body.ApplyLinearImpulse(ref fImpulse);
		}

		public void ApplyAngularImpulse(float impulse)
		{
			Body.ApplyAngularImpulse(impulse);
		}

		public void ApplyTorque(float torque)
		{
			Body.ApplyTorque(torque);
		}

		public Point[] LineVertices
		{
			get
			{
				Transform xf;
				Body.GetTransform(out xf);
				var vertices = new List<Point>();
				foreach (var fixture in Body.FixtureList)
				{
					var shape = fixture.Shape;

					switch (shape.ShapeType)
					{
					case ShapeType.Polygon:
						vertices.AddRange(GetPolygonShapeVertices(shape as PolygonShape, xf));
						break;
					case ShapeType.Edge:
						vertices.AddRange(GetEdgeShapeVertices(shape as EdgeShape, xf));
						break;
					case ShapeType.Circle:
						vertices.AddRange(GetCircleShapeVertices(shape as CircleShape, xf));
						break;
					}
				}

				return vertices.ToArray();
			}
		}

		private IEnumerable<Point> GetPolygonShapeVertices(PolygonShape polygon, Transform xf)
		{
			var vertexCount = polygon.Vertices.Count;
			tempVertices = new Vector2[vertexCount];
			for (int i = 0; i < vertexCount; ++i)
				tempVertices[i] = MathUtils.Mul(ref xf, polygon.Vertices[i]);
			return GetDrawVertices(tempVertices, vertexCount);
		}

		private Vector2[] tempVertices;

		private IEnumerable<Point> GetDrawVertices(Vector2[] vertices, int vertexCount)
		{
			var drawVertices = new List<Point>();
			for (int i = 0; i < (vertexCount - 1); i++)
			{
				drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[i])));
				drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[i + 1])));
			}
			drawVertices.Add(
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[vertexCount - 1])));
			drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[0])));
			return drawVertices.ToArray();
		}

		private IEnumerable<Point> GetEdgeShapeVertices(EdgeShape edge, Transform xf)
		{
			var v1 = MathUtils.Mul(ref xf, edge.Vertex1);
			var v2 = MathUtils.Mul(ref xf, edge.Vertex2);

			return new[]
			{
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(v1)),
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(v2))
			};
		}

		private IEnumerable<Point> GetCircleShapeVertices(CircleShape circle, Transform xf)
		{
			Vector2 center = MathUtils.Mul(ref xf, circle.Position);
			var radius = circle.Radius;
			const int CircleSegments = 32;
			const double Increment = Math.PI * 2.0 / CircleSegments;
			double theta = 0.0;

			var vertices = new List<Point>();
			for (int i = 0; i < CircleSegments; i++)
			{
				Vector2 v1 = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
				Vector2 v2 = center +
					radius *
						new Vector2((float)Math.Cos(theta + Increment), (float)Math.Sin(theta + Increment));

				vertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(v1)));
				vertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(v2)));

				theta += Increment;
			}
			return vertices.ToArray();
		}
	}
}