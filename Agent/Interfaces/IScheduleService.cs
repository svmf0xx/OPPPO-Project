using Agent.AgentModels;
using Shcheduler.Core.Dto;
//using Shcheduler.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Interfaces
{
	public interface IScheduleService
	{
		public IEnumerable<Schedule> GetJobsForRun();
	}
}
