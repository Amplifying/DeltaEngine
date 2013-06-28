using System;
using DeltaEngine.Core;

namespace DeltaEngine.Profiling
{
	public class SystemProfilerSection : IComparable<SystemProfilerSection>
	{
		public SystemProfilerSection(string name)
		{
			Name = name;
			TimeCreated = Time.Current.GetSecondsSinceStartToday();
		}

		public string Name { get; private set; }
		public float TimeCreated { get; private set; }

		public void Log(float value)
		{
			TotalValue += value;
			Calls++;
		}

		public float TotalValue { get; private set; }
		public int Calls { get; private set; }

		public void Reset()
		{
			TotalValue = 0;
			Calls = 0;
		}

		public int CompareTo(SystemProfilerSection other)
		{
			return (int)(other.TotalValue * 1000) - (int)(TotalValue * 1000);
		}
	}
}
