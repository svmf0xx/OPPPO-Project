namespace Shcheduler.Shared
{
	public class AgentAppSettings
	{
		public string apiKey { get; set; }

		public string AgentLocation { get; set; }

		public string AgentName { get; set; } = Environment.MachineName;

		public int UpdateSchTime { get; set; }
	}
}
