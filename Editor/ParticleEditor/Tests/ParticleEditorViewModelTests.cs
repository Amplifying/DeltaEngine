using System;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Particles;
using NUnit.Framework;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.ParticleEditor.Tests
{
	internal class ParticleEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUpParticleEditor()
		{
			mockService = new MockService("TestName", "TestProject");
			viewModel = new ParticleEditorViewModel(mockService);
		}

		private ParticleEditorViewModel viewModel;
		private MockService mockService;

		[Test, CloseAfterFirstFrame]
		public void ViewModelStartsOffWithDefaultEffect()
		{
			Assert.AreEqual("", viewModel.ParticleSystemName);
			Assert.AreEqual(0, viewModel.CurrentEmitterInEffect);
			Assert.NotNull(viewModel.RotationSpeed);
			Assert.NotNull(viewModel.Size);
			Assert.AreEqual(ParticleEmitterPositionType.Point, viewModel.SelectedSpawnerType);
			Assert.AreEqual(0, viewModel.MaxNumberOfParticles);
			Assert.NotNull(viewModel.Color);
			Assert.NotNull(viewModel.StartPosition);
			Assert.NotNull(viewModel.StartRotation);
			Assert.NotNull(viewModel.StartVelocity);
			Assert.NotNull(viewModel.Acceleration);
			Assert.AreEqual(0, viewModel.SpawnInterval);
			Assert.AreEqual(0, viewModel.LifeTime);
			Assert.IsTrue(viewModel.CanModifyEmitters);
			Assert.AreEqual(new[] { 0 }, viewModel.AvailableEmitterIndices);
			Assert.IsTrue(viewModel.SelectedMaterialName.StartsWith("<Generated"));
			Assert.NotNull(viewModel.ParticleEmitterNameToAdd);
		}

		[Test, CloseAfterFirstFrame]
		public void SaveContentTriggersResponseOfService()
		{
			viewModel.ParticleSystemName = "NameOfSavedContent";
			var originalNumberOfMessages = mockService.NumberOfMessagesSent;
			viewModel.Save();
			mockService.ReceiveData(ContentType.ParticleSystem);
			// Saving 1 system + 1 emitter = 2 ContentData
			Assert.AreEqual(originalNumberOfMessages + 2, mockService.NumberOfMessagesSent);
			Assert.IsTrue(Resolve<Logger>().LastMessage.Contains("saved"));
		}

		[Test, CloseAfterFirstFrame]
		public void AddingEmitterIncreasesAvailableIndex()
		{
			var originalMaxIndex = viewModel.AvailableEmitterIndices.Length;
			viewModel.AddEmitterToSystem();
			Assert.AreEqual(originalMaxIndex + 1, viewModel.AvailableEmitterIndices.Length);
		}

		[Test, CloseAfterFirstFrame]
		public void RemovingEmitterDecreasesIndex()
		{
			viewModel.AddEmitterToSystem();
			var originalMaxIndex = viewModel.AvailableEmitterIndices.Length;
			viewModel.RemoveCurrentEmitterFromSystem();
			Assert.AreEqual(originalMaxIndex - 1, viewModel.AvailableEmitterIndices.Length);
		}

		[Test, CloseAfterFirstFrame]
		public void RemovingLastEmitterJustResetsDefaultInstead()
		{
			viewModel.LifeTime = 0.5f;
			viewModel.RemoveCurrentEmitterFromSystem();
			Assert.AreEqual(0.0f, viewModel.LifeTime);
		}

		[Test, CloseAfterFirstFrame]
		public void SettingMaterialNameTriggersEvent()
		{
			Action setName = () => { viewModel.SelectedMaterialName = "TestMaterial"; };
			AssertValueSet(setName);
		}

		private void AssertValueSet(Action settingAction, bool shouldBeSet = true)
		{
			bool valueSet = false;
			viewModel.PropertyChanged += (sender, args) => { valueSet = true; };
			settingAction();
			Assert.AreEqual(shouldBeSet, valueSet);
		}

		[Test, CloseAfterFirstFrame]
		public void WillNotSetEmptyMaterialName()
		{
			Action setEmptyName = () => { viewModel.SelectedMaterialName = ""; };
			AssertValueSet(setEmptyName, false);
		}

		[Test, CloseAfterFirstFrame]
		public void SettingNumberOfParticles()
		{
			Action setNumber = () => { viewModel.MaxNumberOfParticles = 16; };
			AssertValueSet(setNumber);
		}

		[Test, CloseAfterFirstFrame]
		public void SetSpawnerType()
		{
			Action setSpawnType =
				() => { viewModel.SelectedSpawnerType = ParticleEmitterPositionType.Box; };
			AssertValueSet(setSpawnType);
		}

		[Test, CloseAfterFirstFrame]
		public void SetSize()
		{
			Action setSize =
				() => { viewModel.Size = new RangeGraph<Size>(new Size(1.0f), new Size(0.5f)); };
			AssertValueSet(setSize);
		}

		[Test, CloseAfterFirstFrame]
		public void SetColor()
		{
			Action setColor =
				() => { viewModel.Color = new RangeGraph<Color>(Color.Red, Color.Purple); };
			AssertValueSet(setColor);
		}

		[Test, CloseAfterFirstFrame]
		public void SetStartPosition()
		{
			Action setStartPostion =
				() => { viewModel.StartPosition = new RangeGraph<Vector3D>(Vector3D.Zero, Vector3D.Up); };
			AssertValueSet(setStartPostion);
		}

		[Test, CloseAfterFirstFrame]
		public void SetStartRotation()
		{
			Action setStartRotation =
				() =>
				{ viewModel.StartRotation = new RangeGraph<ValueRange>(new ValueRange(), new ValueRange());
				};
			AssertValueSet(setStartRotation);
		}

		[Test, CloseAfterFirstFrame]
		public void SetStartVelocity()
		{
			Action setStartVelocity =
				() => { viewModel.StartVelocity = new RangeGraph<Vector3D>(Vector3D.One, Vector3D.One); };
			AssertValueSet(setStartVelocity);
		}

		[Test, CloseAfterFirstFrame]
		public void SetAcceleration()
		{
			Action setAcceleration =
				() => { viewModel.Acceleration = new RangeGraph<Vector3D>(Vector3D.Zero, Vector3D.One); };
			AssertValueSet(setAcceleration);
		}

		[Test, CloseAfterFirstFrame]
		public void SetCurrentEmitter()
		{
			Action setCurrentEmitter = () => { viewModel.CurrentEmitterInEffect = 0; };
			AssertValueSet(setCurrentEmitter);
		}

		[Test, CloseAfterFirstFrame]
		public void SetRotationSpeed()
		{
			Action setRotationSpeed =
				() =>
				{ viewModel.RotationSpeed = new RangeGraph<ValueRange>(new ValueRange(), new ValueRange());
				};
			AssertValueSet(setRotationSpeed);
		}

		[Test, CloseAfterFirstFrame]
		public void SetSpawnInterval()
		{
			Action setInterval = () => { viewModel.SpawnInterval = 0.2f; };
			AssertValueSet(setInterval);
		}

		[Test, CloseAfterFirstFrame]
		public void ShallNotSaveWithEmptyName()
		{
			viewModel.ParticleSystemName = "";
			viewModel.Save();
			Assert.IsTrue(Resolve<Logger>().LastMessage.Contains("empty name"));
		}

		[Test, CloseAfterFirstFrame]
		public void DeletetionWithoutContentGivesMessage()
		{
			viewModel.ParticleSystemName = "TestContentDeletion";
			viewModel.Delete();
			Assert.IsTrue(Resolve<Logger>().LastMessage.Contains("does not exist"));
		}

		[Test, CloseAfterFirstFrame]
		public void LoadEffect()
		{
			viewModel.ParticleSystemName = "LoadTest";
			viewModel.LoadEffect();
		}

		[Test,CloseAfterFirstFrame]
		public void LoadingEffectWithoutContentGivesFalse()
		{
			Assert.IsFalse(viewModel.TryLoadingEffect(""));
		}

		[Test, CloseAfterFirstFrame]
		public void LookForExistingEmitters()
		{
			AssertValueSet(() => { viewModel.ToggleLookingForExistingEmitters(); });
			Assert.AreEqual(Visibility.Visible ,viewModel.SavedEmitterSelectionVisibility);
		}

		[Test, CloseAfterFirstFrame]
		public void AddExistingEmitter()
		{
			var originalNumberOfEmitters = viewModel.AvailableEmitterIndices.Length;
			viewModel.ParticleEmitterNameToAdd = "SomeEmitterName";
			Assert.AreEqual(originalNumberOfEmitters + 1, viewModel.AvailableEmitterIndices.Length);
			Assert.AreEqual(Visibility.Hidden, viewModel.SavedEmitterSelectionVisibility);
		}

		[Test, CloseAfterFirstFrame]
		public void ToggleSelectionOfExistingTwiceMakesInvisibleAgain()
		{
			AssertValueSet(() => { viewModel.ToggleLookingForExistingEmitters(); });
			AssertValueSet(() => { viewModel.ToggleLookingForExistingEmitters(); });
			Assert.AreEqual(Visibility.Hidden, viewModel.SavedEmitterSelectionVisibility);
		}
	}
}