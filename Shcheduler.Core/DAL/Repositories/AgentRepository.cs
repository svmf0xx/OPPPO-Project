using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shcheduler.Core.DAL.DatabaseModels;
using Shcheduler.Core.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.Repositories
{
    public class AgentRepository : IAgentRepository
    {
        private readonly ShchedulerContext _context;
        public AgentRepository(ShchedulerContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<AgentModel>> GetAllAgentsAsync()
        {
            return await _context.Agents.ToListAsync();
        }

        public async Task<AgentModel> GetAgentByIdAsync(int agentId)
        {
            return await _context.Agents.FindAsync(agentId);
        }

        public async Task<int> AddAgentAsync(AgentModel agent)
        {
            var ag = await _context.Agents.FirstOrDefaultAsync(a => a.ApiKey == agent.ApiKey);
            if (ag == null) 
            {
                _context.Agents.Add(agent);
                await _context.SaveChangesAsync();
                return agent.AgentID;
            }
            return -1;
        }

        public async Task UpdateAgentAsync(AgentModel agent)
        {
            _context.Entry(agent).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAgentAsync(string apiKey)
        {
            var agent = await _context.Agents.FirstOrDefaultAsync(a => a.ApiKey == apiKey);
            if (agent != null)
            {
                _context.Agents.Remove(agent);
                await _context.SaveChangesAsync();
            }
        }

    }
}
