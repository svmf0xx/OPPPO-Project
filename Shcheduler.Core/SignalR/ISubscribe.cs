using Shcheduler.Core;
using Shcheduler.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.SignalR
{
	public interface ISubscribe
	{
		public Task<AgentInitDto> AgentStartConnectionAsync(AgentAppSettings agentAppSettings);
		public Task ClientStartConnectionAsync();
		public Task CloseConnectionAsync(string ApiKey);
    }
}
