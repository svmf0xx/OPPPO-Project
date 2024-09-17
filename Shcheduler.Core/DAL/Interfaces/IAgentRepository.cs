using Shcheduler.Core.DAL.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.Interfaces
{
    public interface IAgentRepository
    {
        Task<IEnumerable<AgentModel>> GetAllAgentsAsync();
        Task<AgentModel> GetAgentByIdAsync(int agentId);
        Task<int> AddAgentAsync(AgentModel agent);
        Task UpdateAgentAsync(AgentModel agent);
        Task RemoveAgentAsync(string apiKey);
    }
}
