using System;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class MockBuilderViewModel : BuilderViewModel
	{
		public MockBuilderViewModel()
			: this(new MockBuilderService()) {}

		public MockBuilderViewModel(Service service)
			: base(service)
		{
			DefineUserSelectedPlatform();
			DefineUserProjectPath();
			DefineUserProjectEntryPointsAndSelectedEntryPoint();
			FakeSomeMessagesIfServiceClientIsMock();
		}

		private void DefineUserSelectedPlatform()
		{
			UserSelectedPlatform = PlatformName.WindowsPhone7;
		}

		private void DefineUserProjectPath()
		{
			UserProjectPath = "LogoApp.sln";
		}

		private void DefineUserProjectEntryPointsAndSelectedEntryPoint()
		{
			const string EntryPoint = "Program.Main";
			UserProjectEntryPoints.Add(EntryPoint);
			UserSelectedEntryPoint = EntryPoint;
		}

		private void FakeSomeMessagesIfServiceClientIsMock()
		{
			var serviceClientMock = service as MockBuilderService;
			if (serviceClientMock != null)
				serviceClientMock.ReceiveSomeTestMessages();
		}

		protected override bool CanBuildExecuted()
		{
			return true;
		}

		protected override void OnBuildExecuted()
		{
			Console.WriteLine(UserProjectPath + " built.");
		}
	}
}