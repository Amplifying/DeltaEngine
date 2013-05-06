using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Editor.Common
{
	/// <summary>
	/// Basic content service for the contentmanager
	/// </summary>
	public interface ContentService
	{
		void AddProject(string projectName);
		List<string> GetProjects();
		List<string> GetContentNames(string projectName);
		//TODO: also give extension for type detection
		void AddContent(string projectName, string contentName, Stream data);
		void DeleteContent(string projectName, string contentName);
		Stream LoadContent(string projectName, string contentName);
	}
}