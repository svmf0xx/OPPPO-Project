using Agent.AgentModels;
using Shcheduler.Core;
using Shcheduler.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Interfaces
{
	public interface ISignalRAgent
	{
		public Task<AgentInitDto> StartConnectionAsync(AgentAppSettings agentSettings);
		public Task SendResponse(ResponseInWebDto resp);
		public Task UpdExTime(Schedule sch);
		public Task StopAgent(string ApiKey);
		public Task SendTimeTable(TimeTableDto timetable);
    }
}
