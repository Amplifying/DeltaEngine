namespace DeltaEngine.Editor.Messages
{
	public class AppBuildProgress
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		protected AppBuildProgress() {} // ncrunch: no coverage

		public AppBuildProgress(string text, int progressPercentage)
		{
			Text = text;
			ProgressPercentage = progressPercentage;
		}

		public string Text { get; private set; }
		public int ProgressPercentage { get; private set; }

		// ncrunch: no coverage start
		public override string ToString()
		{
			return ProgressPercentage + "% - " + Text;
		}
	}
}