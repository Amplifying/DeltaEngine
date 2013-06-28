using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Dark
{
	public class SceneLevel : Scene
	{
		public SceneLevel(Game game, InputCommands input, ScreenSpace screenSpace)
			: base(game, input, screenSpace)
		{
			camera = screenSpace as Camera2DControlledQuadraticScreenSpace;
		}

		private readonly Camera2DControlledQuadraticScreenSpace camera;

		public override void Load()
		{
			LoadBackground();
			LoadFlashlight();
			LoadLevelObjects();
			LoadMainCharacter();
			LoadEnemies();
			SetupInput();
			SetupCamera();
			SetupStartPoint();
		}

		private void SetupCamera()
		{
			cameraPosition = Vector.Zero;
			cameraFacing = Vector.Zero;
			cameraRotation = 0.0f;
			camera.RotationCenter = new Point(0.5f, 0.8f);
			camera.Zoom = 1.0f;
		}

		private Vector cameraPosition;
		private Vector cameraFacing;
		private float cameraRotation;

		private void SetupInput()
		{
			input.Add(Key.W, State.Pressed, key => MoveUp());
			input.Add(Key.S, State.Pressed, key => MoveDown());
			input.Add(Key.A, State.Pressed, key => MoveLeft());
			input.Add(Key.D, State.Pressed, key => MoveRight());
			input.Add(Key.W, State.Released, key => StopUp());
			input.Add(Key.S, State.Released, key => StopDown());
			input.Add(Key.A, State.Released, key => StopLeft());
			input.Add(Key.D, State.Released, key => StopRight());
			input.Add(MouseButton.Left, State.Released, UpdateRotation);
			input.Add(MouseButton.Left, State.Pressing, m => camera.Zoom *= 2.0f);
			input.Add(MouseButton.Right, State.Pressing, m => camera.Zoom *= 0.5f);
		}

		private void UpdateRotation(Mouse mouse)
		{
			var rotationSpeed = -mouse.Position.X + camera.Left + (0.5f / camera.Zoom);
			rotationSpeed = rotationSpeed.Abs() < MinimumMousePositionToRotate ? 0 : rotationSpeed;
			cameraRotation += rotationSpeed * Time.Current.Delta * CameraRotation * camera.Zoom;
			cameraFacing = GetFacingVector(cameraRotation);
			mainCharacter.Facing = cameraFacing;
			flashLight.Facing = cameraFacing;
		}

		private const float MinimumMousePositionToRotate = 0.10f;
		private const float CameraRotation = 100.0f;

		private static Vector GetFacingVector(float angle)
		{
			return
				Vector2DMath.Normalize(new Vector(-MathExtensions.Sin(angle), -MathExtensions.Cos(angle),
					0.0f));
		}

		private void LoadBackground()
		{
			background = new LevelBackground(ContentLoader.Load<Image>("AsylumFull"),
				new Rectangle(0.0f, 0.0f, BackgroundWorldWidth, BackgroundWorldHeight));
		}

		private LevelBackground background;
		private const float BackgroundWorldWidth = 8.192f;
		private const float BackgroundWorldHeight = 5.432f;

		private void LoadFlashlight()
		{
			flashLight = new Flashlight(ContentLoader.Load<Image>("Flashlight01"), Vector.Zero, content);
		}

		private Flashlight flashLight;

		private void LoadLevelObjects()
		{
			var levelObjectXmlParser = new LevelObjectXmlParser();
			var levelObjectsXmlList = levelObjectXmlParser.ParseXml();
			foreach (var levelObjectXml in levelObjectsXmlList)
				AddLevelObject(levelObjectXml);
		}

		private readonly List<LevelObject> levelObjects = new List<LevelObject>();

		private void AddLevelObject(LevelObjectDefinition levelObjectDefinition)
		{
			var fixedLevelObjectPosition = new Point(
				levelObjectDefinition.Position.X + ReferenceOffsetX,
				levelObjectDefinition.Position.Y + ReferenceOffsetY);
			var objectPosition = Coordinates.PixelToWorld(fixedLevelObjectPosition);
			//var levelObject = new LevelObject(content.Load<Image>(levelObjectDefinition.Filename),
			//var levelObject = new LevelObject(content.Load<Image>("DeltaEngineLogo"),
			var levelObject = new LevelObject(ContentLoader.Load<Image>("transparent"),
				new Vector(objectPosition.X, objectPosition.Y, 0.0f),
				new Size(levelObjectDefinition.Width * 0.01f, levelObjectDefinition.Height * 0.01f),
				levelObjectDefinition.Rotation);
			levelObjects.Add(levelObject);
		}

		private const float ReferenceOffsetX = -19.0f;
		private const float ReferenceOffsetY = 62.0f;

		private void LoadMainCharacter()
		{
			mainCharacter = new MainCharacter(ContentLoader.Load<Image>("AsylumChar112px01"), Vector.Zero);
		}

		private MainCharacter mainCharacter;

		private void LoadEnemies()
		{
			var enemyWaypointXmlParser = new EnemyWaypointXmlParser(content);
			var enemyWaypointLists = enemyWaypointXmlParser.ParseXml();
			foreach (var enemyWaypointList in enemyWaypointLists)
			{
				var enemy = new Enemy(ContentLoader.Load<Image>("AsylumBoy112px01"), content, enemyWaypointList);
				enemies.Add(enemy);
			}
		}

		private readonly List<Enemy> enemies = new List<Enemy>();

		private void SetupStartPoint()
		{
			cameraPosition = new Vector(StartPointX, StartPointY - 0.3f, 0.0f);
			cameraRotation = 180.0f;
			mainCharacter.Position = new Vector(StartPointX, StartPointY, 0.0f);
			flashLight.Position = mainCharacter.Position;
		}

		private const float StartPointX = 2.8f;
		private const float StartPointY = 1.4f;

		public override void Update()
		{
			camera.LookAt = new Point(cameraPosition.X, cameraPosition.Y);
			camera.Rotation = cameraRotation;
			mainCharacter.Update();
			flashLight.Update();
			foreach (var enemy in enemies)
				enemy.Update();
		}

		private void MoveUp()
		{
			movingUp = true;
			mainCharacter.State = CharacterState.Moving;
			flashLight.State = CharacterState.Moving;
			MoveIfPossible(cameraFacing * (Time.Current.Delta * CameraMovement));
		}

		private const float CameraMovement = 0.5f;
		private bool movingUp;

		private void MoveIfPossible(Vector positionIncrement)
		{
			if (CanMoveTo(mainCharacter.Position + positionIncrement))
				MoveCameraAndMainCharacter(positionIncrement);
		}

		private bool CanMoveTo(Vector position)
		{
			var color = background.GetBitmapPixel(position);
			if (color.R == 0 && color.G == 0 && color.B == 0)
				return false;

			foreach (var levelObject in levelObjects)
				if (IsInsideObject(levelObject, position))
					return false;

			return true;
		}

		private bool IsInsideObject(LevelObject levelObject, Vector position)
		{
			var distance = Vector2DMath.GetLength(position - levelObject.Position);
			var objectSizes = levelObject.DrawArea.Size.Width * 0.5f +
				mainCharacter.DrawArea.Size.Height * 0.5f;
			return distance <= objectSizes;
		}

		private void MoveCameraAndMainCharacter(Vector positionIncrement)
		{
			cameraPosition += positionIncrement;
			mainCharacter.Position += positionIncrement;
			flashLight.Position += positionIncrement;
			RenderShadow.LightPosition = new Point(mainCharacter.Position.X, mainCharacter.Position.Y);
		}

		private void MoveDown()
		{
			movingDown = true;
			mainCharacter.State = CharacterState.Moving;
			flashLight.State = CharacterState.Moving;
			MoveIfPossible(-cameraFacing * (Time.Current.Delta * CameraMovement));
		}

		private bool movingDown;

		private void MoveLeft()
		{
			movingLeft = true;
			mainCharacter.State = CharacterState.Moving;
			flashLight.State = CharacterState.Moving;
			var turnVector = new Vector(cameraFacing.Y, -cameraFacing.X, 0.0f);
			MoveIfPossible(turnVector * (Time.Current.Delta * CameraMovement));
		}

		private bool movingLeft;

		private void MoveRight()
		{
			movingRight = true;
			mainCharacter.State = CharacterState.Moving;
			flashLight.State = CharacterState.Moving;
			var turnVector = new Vector(-cameraFacing.Y, cameraFacing.X, 0.0f);
			MoveIfPossible(turnVector * (Time.Current.Delta * CameraMovement));
		}

		private bool movingRight;

		private void StopUp()
		{
			movingUp = false;
			StopMainCharacter();
		}

		private void StopMainCharacter()
		{
			if (!movingUp && !movingDown && !movingLeft && !movingRight)
			{
				mainCharacter.State = CharacterState.Stopped;
				flashLight.State = CharacterState.Stopped;
			}
		}

		private void StopDown()
		{
			movingDown = false;
			StopMainCharacter();
		}

		private void StopLeft()
		{
			movingLeft = false;
			StopMainCharacter();
		}

		private void StopRight()
		{
			movingRight = false;
			StopMainCharacter();
		}

		public override void Release()
		{
			background.IsActive = false;
		}
	}
}