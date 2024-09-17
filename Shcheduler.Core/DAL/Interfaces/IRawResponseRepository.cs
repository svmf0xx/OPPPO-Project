using Shcheduler.Core.DAL.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.Interfaces
{
    public interface IRawResponseRepository
    {
        Task<IEnumerable<RawResponseModel>> GetAllRawResponsesAsync();
        Task<RawResponseModel> GetRawResponseByIdAsync(int responseId);
        Task AddRawResponseAsync(RawResponseModel rawResponse);
        Task UpdateRawResponseAsync(RawResponseModel rawResponse);
        Task RemoveRawResponseAsync(int responseId);
    }
}
