using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agent.Interfaces;
using Shcheduler.Core.Dto;
using Shcheduler.Shared;
using Agent.AgentModels;
using System.Net;
namespace Agent.Realizations
{
	public class ScheduleService(ILogger<ScheduleService> logger, IJobService jobService): IScheduleService
	{
		private readonly ILogger<ScheduleService> _logger = logger;
		private readonly IJobService _jobService = jobService;

		public IEnumerable<Schedule> GetJobsForRun()
		{
			foreach (var job in _jobService.GetAllJobs())
			{
				yield return job;
			}
		}
	}
}
