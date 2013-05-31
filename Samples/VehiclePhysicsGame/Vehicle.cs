using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;

namespace VehiclePhysicsGame
{
	public class Vehicle : Entity
	{
		public Vehicle(Point initialPosition, Physics physics, ContentLoader contentLoader)
		{
			this.physics = physics;

			CreateCarBodySpriteAndPhysicsBody(initialPosition);
			CreateWheelSpritesAndPhysicsBodies(initialPosition);
			//CreateSuspensionJoints();
			Add<VehicleHandler>();
		}

		private readonly Physics physics;

		protected void CreateCarBodySpriteAndPhysicsBody(Point initialPosition)
		{
			var size = new Size(BodyLength, BodyHeight);
			chassis = new Rect(Rectangle.FromCenter(initialPosition,size), Color.TransparentBlack);
			chassis.Add(new OutlineColor(Color.Green));
			chassis.Add<Polygon.RenderOutline>();
			var chassisPhysicsBody = physics.CreateRectangle(size);
			chassisPhysicsBody.Position = initialPosition;
			//chassis.Add(chassisPhysicsBody);
			//chassis.Add<Physics2D>();
		}

		protected void CreateWheelSpritesAndPhysicsBodies(Point initialPosition)
		{
			var wheelSize = new Size(WheelRadius * 2);

			var frontWheelPosition = new Point(initialPosition.X + WheelbaseFront,
				initialPosition.Y + SuspensionHeight);
			frontWheel = new Ellipse(frontWheelPosition,WheelRadius,WheelRadius,Color.DarkGray);
			var frontWheelPhysicsBody = physics.CreateCircle(WheelRadius);
			frontWheelPhysicsBody.Position = frontWheelPosition;
			frontWheelPhysicsBody.Friction = 0.9f;
			//frontWheel.Add(frontWheelPhysicsBody);
			//frontWheel.Add<Physics2D>();

			var rearWheelPosition = new Point(initialPosition.X - WheelbaseRear,
				initialPosition.Y + SuspensionHeight);
			rearWheel = new Ellipse(rearWheelPosition, WheelRadius, WheelRadius, Color.DarkGray);
			var rearWheelPhysicsBody = physics.CreateCircle(WheelRadius);
			rearWheelPhysicsBody.Position = rearWheelPosition;
			rearWheelPhysicsBody.Friction = 0.9f;
			//rearWheel.Add(rearWheelPhysicsBody);
			//rearWheel.Add<Physics2D>();
		}

		private void CreateSuspensionJoints()
		{
			frontSuspension = physics.CreateLineJoint(chassis.Get<PhysicsBody>(),
				frontWheel.Get<PhysicsBody>(), new Point(0, -SuspensionHeight));
			frontSuspension.MotorSpeed = 0.0f;
			frontSuspension.MaxMotorTorque = 1.0f;
			frontSuspension.MotorEnabled = false;
			frontSuspension.Frequency = 0.085f;

			rearSuspension = physics.CreateLineJoint(chassis.Get<PhysicsBody>(),
				rearWheel.Get<PhysicsBody>(), new Point(0, -SuspensionHeight));
			rearSuspension.MotorSpeed = 0.0f;
			rearSuspension.MaxMotorTorque = 2.0f;
			rearSuspension.MotorEnabled = true;
			rearSuspension.Frequency = 0.05f;
		}

		protected Polygon chassis;
		protected Ellipse frontWheel, rearWheel;
		protected PhysicsJoint frontSuspension, rearSuspension;
		private const float BodyLength = 0.3f, BodyHeight = 0.1f, WheelRadius = 0.03f,
			WheelbaseFront = 0.09f, WheelbaseRear = 0.07f, SuspensionHeight = 0.06f;
		
		public void Accelerate()
		{
			
		}

		public void Brake()
		{
			
		}

		public Point Position { get { return chassis.Get<Rectangle>().Center; } }
	}

	internal class VehicleHandler : EntityHandler
	{
		public override void Handle(List<Entity> entities)
		{
			foreach (Vehicle entity in entities)
			{
				
			}
		}
	}
}