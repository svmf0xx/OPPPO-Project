using Agent.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Shcheduler.Core;
using Shcheduler.Core.Dto;
using Shcheduler.Core.SignalR;
using Agent.AgentModels;
using Microsoft.EntityFrameworkCore.Metadata;
namespace Agent.Realizations
{
	public class SignalRAgent(IConfiguration config, HubConnection hubConnection, ILogger<SignalRAgent> logger, AgentAppSettings appSettings) : ISignalRAgent
	{
		private readonly HubConnection _hubConnection = hubConnection;
		private readonly ILogger<SignalRAgent> _logger = logger;
		private readonly AgentAppSettings _appSettings = appSettings;
		public async Task<AgentInitDto> StartConnectionAsync(AgentAppSettings agentSettings)
		{
			if (_hubConnection.State != HubConnectionState.Connected)
			{
				await _hubConnection.StartAsync();
			}
			agentSettings.AgentName = "ServerAgent";
			var init = await _hubConnection.InvokeAsync<AgentInitDto>(nameof(ISubscribe.AgentStartConnectionAsync), agentSettings);
			
			return init;
		}
		public async Task SendResponse(ResponseInWebDto resp)
		{
			await _hubConnection.SendAsync(nameof(IRecieve.RecieveResponse), resp);
		}

		public async Task UpdExTime(Schedule sch)
		{
			var result = new JobResultDto { SchId = sch.BigJobId, NextEx = sch.NextEx, LastEx = sch.LastEx, Status = sch.Status};
			await _hubConnection.SendAsync(nameof(IRecieve.RecieveExecutionTime), result);
		}
		public async Task StopAgent(string apiKey)
		{
			await _hubConnection.SendAsync(nameof(ISubscribe.CloseConnectionAsync), apiKey);
		}
		public async Task SendTimeTable(TimeTableDto timetable)
		{
            await _hubConnection.SendAsync(nameof(IRecieve.RecieveTimeTable), timetable);
        }
	}
}
