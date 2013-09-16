using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Mocks
{
	public class MockService : Service
	{
		public MockService(string userName, string projectName)
		{
			UserName = userName;
			ProjectName = projectName;
		}

		public string UserName { get; private set; }
		public string ProjectName { get; private set; }

		public void ReceiveData(object data)
		{
			if (DataReceived != null)
				DataReceived(data);
			if (ContentUpdated != null)
				ContentUpdated(ContentType.Scene, "MockContent");
			if (ContentDeleted != null)
				ContentDeleted("MockContent");
		}

		public event Action<object> DataReceived;
		public event Action<ContentType, string> ContentUpdated;
		public event Action<string> ContentDeleted;

		public void Send(object message) {}

		public IEnumerable<string> GetAllContentNames()
		{
			var list = new List<string>();
			list.Add("Test1");
			list.Add("Test2");
			return list;
		}

		public IEnumerable<string> GetAllContentNamesByType(ContentType type)
		{
			var list = new List<string>();
			list.Add("Test1");
			list.Add("Test2");
			return list;
		}

		public ContentType? GetTypeOfContent(string content)
		{
			if ("TestUser" == content)
				return null;
			return ContentType.Image;
		}

		public void UploadContent(ContentMetaData metaData,
			Dictionary<string, byte[]> optionalFileData = null) {}

		public void DeleteContent(string selectedContent, bool deleteSubContent) {}

		public void StartPlugin(Type plugin) {}
	}
}