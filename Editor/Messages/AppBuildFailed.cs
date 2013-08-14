namespace DeltaEngine.Editor.Messages
{
	public sealed class AppBuildFailed
	{
		private AppBuildFailed() {}

		public AppBuildFailed(string reason)
		{
			Reason = reason;
		}

		public string Reason { get; private set; }
	}
}