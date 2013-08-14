using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class MockBuildService : Service
	{
		public MockBuildService()
		{
			UserName = GetType().Name + "User";
			ProjectName = "LogoApp";
			Platforms = new[] { PlatformName.Windows };
		}

		public string UserName { get; private set; }
		public string ProjectName { get; private set; }
		public PlatformName[] Platforms { get; private set; }
		public event Action<object> DataReceived;
		public event Action ContentChanged;

		public void Send(object message)
		{
			if (message is AppBuildRequest)
				OnHandleBuildRequest((AppBuildRequest)message);
		}

		private void OnHandleBuildRequest(AppBuildRequest userBuildRequest)
		{
			ReceiveData(new AppBuildMessage("Finished with fake build."));
			ReceiveData(new AppBuildResult(userBuildRequest.ProjectName, userBuildRequest.Platform));
		}

		public void ReceiveData(object fakedReceivedData)
		{
			DataReceived(fakedReceivedData);
		}

		public void ChangeContent()
		{
			ContentChanged();
		}

		public void ReceiveSomeTestMessages()
		{
			ReceiveData("A BuildWarning".AsBuildTestWarning());
			ReceiveData("A BuildError".AsBuildTestError());
		}

		public IEnumerable<string> GetAllContentNames()
		{
			return new List<string>();
		}

		public IEnumerable<string> GetAllContentNamesByType(ContentType type)
		{
			return new List<string>();
		}

		public ContentType? GetTypeOfContent(string content)
		{
			return ContentType.Image;
		}

		public void UploadContent(ContentMetaData metaData,
			Dictionary<string, byte[]> optionalFileData = null) {}

		public void DeleteContent(string selectedContent) {}
	}
}