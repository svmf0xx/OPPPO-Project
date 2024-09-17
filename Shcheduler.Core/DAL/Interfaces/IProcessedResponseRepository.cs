using Shcheduler.Core.DAL.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.Interfaces
{
    public interface IProcessedResponseRepository
    {
        Task<IEnumerable<ProcessedResponseModel>> GetAllProcessedResponsesAsync();
        Task<ProcessedResponseModel> GetProcessedResponseByIdAsync(int processedResponseId);
        Task AddProcessedResponseAsync(ProcessedResponseModel processedResponse);
        Task UpdateProcessedResponseAsync(ProcessedResponseModel processedResponse);
        Task RemoveProcessedResponseAsync(int processedResponseId);
    }
}
