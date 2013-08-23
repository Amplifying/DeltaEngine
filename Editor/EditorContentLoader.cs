using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Online;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Editor
{
	public class EditorContentLoader : DeveloperOnlineContentLoader
	{
		public EditorContentLoader(OnlineServiceConnection connection)
			: base(connection)
		{
			isIgnoringSetProject = true;
		}

		public EditorContentLoader(OnlineServiceConnection connection, SetProject newProject)
			: base(connection, Path.Combine("Content", newProject.ProjectName))
		{
			isIgnoringSetProject = true;
			VerifyProjectAndGetContent(newProject);
			RefreshMetaData();
		}

		public List<string> GetAllNames()
		{
			RefreshMetaData();
			return new List<string>(metaData.Keys);
		}

		public List<string> GetAllNamesByType(ContentType type)
		{
			RefreshMetaData();
			var names = new List<string>();
			foreach (var contentMetaData in metaData)
				if (contentMetaData.Value.Type == type)
					names.Add(contentMetaData.Key);
			return names;
		}

		public void Upload(ContentMetaData contentMetaData,
			Dictionary<string, byte[]> optionalFileData)
		{
			var fileList = new List<UpdateContent.FileNameAndBytes>();
			if (optionalFileData != null)
				foreach (var file in optionalFileData)
					fileList.Add(new UpdateContent.FileNameAndBytes { name = file.Key, data = file.Value });
			connection.Send(new UpdateContent(contentMetaData, fileList.ToArray()));
		}

		public void Delete(string contentName, bool deleteSubContent)
		{
			connection.Send(new DeleteContent(contentName, deleteSubContent));
		}

		public ContentType? GetTypeOfContent(string content)
		{
			foreach (var contentMetaData in metaData)
				if (contentMetaData.Value.Name == content)
					return contentMetaData.Value.Type;
			return null;
		}
	}
}