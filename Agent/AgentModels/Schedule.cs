using Shcheduler.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.AgentModels
{
	public class Schedule
	{
		public int BigJobId { get; set; }
		public string JobName { get; set; }
		public string apiKey { get; set; }
		public List<JobDto> Jobs { get; set; }
		public string Cron { get; set; }
		public bool StoppingByError {  get; set; }
		public string? Timezone { get; set; }
		public string? Status { get; set; }
		public DateTime LastEx {  get; set; }
		public DateTime NextEx { get; set; }
	}

}
