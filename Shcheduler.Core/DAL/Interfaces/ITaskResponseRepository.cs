using Shcheduler.Core.DAL.DatabaseModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.Interfaces
{
    public interface ITaskResponseRepository
    {
        TaskResponseModel GetById(int id);
        IEnumerable<TaskResponseModel> GetAll();
        Task Add(TaskResponseModel taskResponse);
        void Update(TaskResponseModel taskResponse);
        void Delete(TaskResponseModel taskResponse);
        Task<IEnumerable<TaskResponseModel>> GetLastTaskResponses();
        Task<IEnumerable<TaskResponseModel>> GetTaskResponsesByIdAsync(int taskId);
        Task<(int TotalPages, IEnumerable<TaskResponseModel> TaskResponses)> GetTaskResponsesByIdAndPageAsync(int taskId, int page);
        Task<(int TotalPages, IEnumerable<TaskResponseModel> TaskResponses)> GetTaskResponsesByIdAndPageAndSearchDateTimeAsync(int taskId, int page, DateOnly? searchDate);
        Task<MemoryStream> ArchiveLogsForJobAsync(int jobId, bool deleteLogs);
    }
}
