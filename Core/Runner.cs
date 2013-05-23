namespace DeltaEngine.Core
{
	/// <summary>
	/// Allows classes to automatically run before any app code each frame (clearing, actors, physics,
	/// etc.). Runners are executed in threads based on dependencies used in their constructors.
	/// http://DeltaEngine.net/About/CodingStyle#Runners
	/// </summary>
	public interface Runner
	{
		void Run();
	}

	/// <summary>
	/// Variation of Runner to allow passing in any dependency automatically into Run.
	/// </summary>
	public interface Runner<in FirstDependency>
	{
		void Run(FirstDependency first);
	}

	/// <summary>
	/// Variation of Runner to allow passing in two dependencies automatically into Run.
	/// </summary>
	public interface Runner<in FirstDependency, in SecondDependency>
	{
		void Run(FirstDependency first, SecondDependency second);
	}

	/// <summary>
	/// Variation of Runner to allow passing in three dependencies automatically into Run.
	/// </summary>
	public interface Runner<in FirstDependency, in SecondDependency, in ThirdDependency>
	{
		void Run(FirstDependency first, SecondDependency second, ThirdDependency third);
	}
}