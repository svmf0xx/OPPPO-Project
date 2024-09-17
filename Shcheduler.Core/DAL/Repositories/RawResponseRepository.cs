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
    public class RawResponseRepository:IRawResponseRepository
    {
        private readonly ShchedulerContext _context;
        public RawResponseRepository(ShchedulerContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RawResponseModel>> GetAllRawResponsesAsync()
        {
            return await _context.RawResponses.ToListAsync();
        }

        public async Task<RawResponseModel> GetRawResponseByIdAsync(int responseId)
        {
            return await _context.RawResponses.FindAsync(responseId);
        }

        public async Task AddRawResponseAsync(RawResponseModel rawResponse)
        {
            _context.RawResponses.Add(rawResponse);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRawResponseAsync(RawResponseModel rawResponse)
        {
            _context.Entry(rawResponse).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRawResponseAsync(int responseId)
        {
            var rawResponse = await _context.RawResponses.FindAsync(responseId);
            if (rawResponse != null)
            {
                _context.RawResponses.Remove(rawResponse);
                await _context.SaveChangesAsync();
            }
        }
    }
}
