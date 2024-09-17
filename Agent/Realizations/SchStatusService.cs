using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agent.AgentModels;
using Agent.Interfaces;
using Shcheduler.Core.Dto;
using Shcheduler.Shared;

namespace Agent.Realizations
{
    public class SchStatusService(ILogger<SchStatusService> logger) : ISchStatusService
	{
		private readonly ILogger<SchStatusService> _logger = logger;
		public List<Task<ResponseInWebDto>> CurrentRunningTasks = new List<Task<ResponseInWebDto>>();
		public void AddToRunning(Task<ResponseInWebDto> task)
		{
			CurrentRunningTasks.Add(task);
		}

		public List<Task<ResponseInWebDto>> GetCurrTasks()
		{
			return CurrentRunningTasks;
		}
	}
}
