using System;
using System.Collections.Generic;
using DeltaEngine.Content;

namespace DeltaEngine.Editor.Core
{
	public interface Service
	{
		string UserName { get; }
		string ProjectName { get; }
		event Action<object> DataReceived;
		event Action<ContentType, string> ContentUpdated;
		event Action<string> ContentDeleted;
		void Send(object message, bool allowToCompressMessage = true);
		IEnumerable<string> GetAllContentNames();
		IEnumerable<string> GetAllContentNamesByType(ContentType type);
		ContentType? GetTypeOfContent(string content);
		void UploadContent(ContentMetaData metaData, Dictionary<string, byte[]> optionalFileData = null);
		void DeleteContent(string selectedContent, bool deleteSubContent = false);
		void StartPlugin(Type plugin);
	}
}