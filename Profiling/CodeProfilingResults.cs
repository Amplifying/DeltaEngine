using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Profiling
{
	/// <summary>
	/// The results of a session of profiling
	/// </summary>
	public class CodeProfilingResults
	{
		public CodeProfilingResults(List<CodeProfilerSection> sections)
		{
			Sections = sections;
			TotalTime = Time.Current.GetSecondsSinceStartToday();
			TotalSectionTime = MathExtensions.Min(Sections.Sum(time => time.TotalTime), TotalTime);
		}

		public List<CodeProfilerSection> Sections { get; private set; }
		public float TotalTime { get; private set; }
		public float TotalSectionTime { get; private set; }
	}
}