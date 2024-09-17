using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Shcheduler.Core.DAL.Interfaces;
using Shcheduler.Core.Dto;
using Shcheduler.Core.DAL.DatabaseModels;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata;
namespace Shcheduler.Core.SignalR
{
	public class SignalRHub(ITaskResponseRepository responseRep, IConverter converter, IConfiguration config, ITaskRepository taskRep, IAgentRepository agentRepository) :Hub<ISend>, IRecieve, ISubscribe
	{
		private readonly ITaskResponseRepository _responseRep = responseRep;
		private readonly IConverter _converter = converter;
		private readonly IConfiguration _config = config;
		private readonly ITaskRepository _taskRepository = taskRep;
		private readonly IAgentRepository _agentRepository = agentRepository;
		public async Task<AgentInitDto> AgentStartConnectionAsync(AgentAppSettings agentAppSettings)
		{
			var init = new AgentInitDto();
			//agentAppSettings.ApiKey = new Random().Next().ToString(); на потом


			agentAppSettings.ApiKey = agentAppSettings.AgentName;
            _ = await _agentRepository.AddAgentAsync(_converter.Convert(agentAppSettings));

            await this.Clients.Group(_config.GetSection("SignalRGroupNames:ClientsGroupName").Value).UpdateAgentsForClient(agentAppSettings.ApiKey);
            init.ApiKey = agentAppSettings.ApiKey;

			await Groups.AddToGroupAsync(Context.ConnectionId, _config.GetSection("SignalRGroupNames:AgentsGroupName").Value);
			Context.Items["apiKey"] = agentAppSettings.ApiKey;
			var schedule = _converter.Convert(await _taskRepository.GetAllTasksAsync()).ToList();
			foreach (var scheduleItem in schedule)
			{
				scheduleItem.Jobs = _converter.ConvertJobs(await _taskRepository.GetAllJobsForTaskAsync(scheduleItem.BigJobId)).ToList();
			}
			init.Schedule = schedule;
			return init;
		}
		public async Task ClientStartConnectionAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, _config.GetSection("SignalRGroupNames:ClientsGroupName").Value);
		}

		public async Task RecieveExecutionTime(JobResultDto result)
		{
			await this.Clients.Group(_config.GetSection("SignalRGroupNames:ClientsGroupName").Value).SendExecutionTime(result);
		}
		public async Task RecieveResponse(ResponseInWebDto resp)
		{
			//запись в бд
			var jobresponses = new List<JobResponseModel>();
			foreach (var job in resp.ResponsesList)
			{
				jobresponses.Add(new JobResponseModel()
				{
					JobId = job.JobID,
					Status = job.Status,
					Body = job.Body,
					ExecutionDuration = job.ExecutionDuration,
                    RegExError = job.RegExError,
                    Header = job.Header
				});
			}
			var responseToDb = new TaskResponseModel()
			{
				TaskId = resp.BigJobID,
				Status = resp.Status,
				ExecutionDuration = resp.ExecutionDuration,
				JobResponses = jobresponses,
				EndTime = resp.EndTime,
				StartTime = resp.StartTime
			};
			await _responseRep.Add(responseToDb);


			//отправка клиенту по сокетам
			await this.Clients.Group(_config.GetSection("SignalRGroupNames:ClientsGroupName").Value).SendResponse(resp);
		}

		public async Task CheckStatus(int id)
        {
            await this.Clients.Group(_config.GetSection("SignalRGroupNames:AgentsGroupName").Value).CheckStatus(id);
        }
		public async Task ExecuteNow(int id)
		{
			await this.Clients.Group(_config.GetSection("SignalRGroupNames:AgentsGroupName").Value).ExecuteNow(id);
		}
		public async Task CloseConnectionAsync(string apiKey)
		{
			await _agentRepository.RemoveAgentAsync(apiKey);
		}
        public async Task ExecTimeTable(int id)
        {
            await this.Clients.Group(_config.GetSection("SignalRGroupNames:AgentsGroupName").Value).ExecTimeTable(id);
        }
		public async Task RecieveTimeTable(TimeTableDto timetable)
		{
            await this.Clients.Group(_config.GetSection("SignalRGroupNames:ClientsGroupName").Value).SendTimeTableToClient(timetable);
        }
    
        public override async Task OnDisconnectedAsync(Exception exception)
		{
			if (Context.Items.TryGetValue("apiKey", out var apiKey))
			{
				await _agentRepository.RemoveAgentAsync(apiKey.ToString());
				await this.Clients.Group(_config.GetSection("SignalRGroupNames:ClientsGroupName").Value).UpdateAgentsForClient(apiKey.ToString());
			}
			
			await base.OnDisconnectedAsync(exception);
		}
	}
}
