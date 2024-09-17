using Agent.AgentModels;
using Microsoft.Identity.Client;
using Shcheduler.Core.Dto;
//using Shcheduler.Shared;

namespace Agent.Interfaces
{
    public interface ISchStatusService
    {
		public List<Task<ResponseInWebDto>> GetCurrTasks();
		public void AddToRunning(Task<ResponseInWebDto> task);

	}
}