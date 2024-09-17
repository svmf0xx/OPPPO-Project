using Microsoft.EntityFrameworkCore;
using Shcheduler.Core.DAL.DatabaseModels;
using Shcheduler.Core.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.Repositories
{
    public class ProcessedResponseRepository:IProcessedResponseRepository
    {
        private readonly ShchedulerContext _context;
        public ProcessedResponseRepository(ShchedulerContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ProcessedResponseModel>> GetAllProcessedResponsesAsync()
        {
            return await _context.ProcessedResponses.ToListAsync();
        }

        public async Task<ProcessedResponseModel> GetProcessedResponseByIdAsync(int processedResponseId)
        {
            return await _context.ProcessedResponses.FindAsync(processedResponseId);
        }

        public async Task AddProcessedResponseAsync(ProcessedResponseModel processedResponse)
        {
            _context.ProcessedResponses.Add(processedResponse);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProcessedResponseAsync(ProcessedResponseModel processedResponse)
        {
            _context.Entry(processedResponse).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProcessedResponseAsync(int processedResponseId)
        {
            var processedResponse = await _context.ProcessedResponses.FindAsync(processedResponseId);
            if (processedResponse != null)
            {
                _context.ProcessedResponses.Remove(processedResponse);
                await _context.SaveChangesAsync();
            }
        }
    }
}
