using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shcheduler.Core.DAL.DatabaseModels;
using Shcheduler.Core.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.Repositories
{
    public class TaskResponseRepository:ITaskResponseRepository
    {
        private readonly ShchedulerContext _context;
        private readonly ILogger<TaskResponseRepository> _logger;

        public TaskResponseRepository(ShchedulerContext context, ILogger<TaskResponseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Add(TaskResponseModel taskResponse)
        {
            _context.TaskResponses.Add(taskResponse);
            await _context.SaveChangesAsync(); // Сохраняем TaskResponse и получаем его Id

            int taskResponseId = taskResponse.Id;

            foreach (var jobResponse in taskResponse.JobResponses)
            {
                jobResponse.TaskResponseId = taskResponseId;
                _context.JobResponses.Add(jobResponse);
            }

            await _context.SaveChangesAsync(); // Сохраняем JobResponses
        }

        public void Delete(TaskResponseModel taskResponse)
        {
            _context.TaskResponses.Remove(taskResponse);
            _context.SaveChanges();
        }

        public IEnumerable<TaskResponseModel> GetAll()
        {
            return _context.TaskResponses.ToList();
        }

        public TaskResponseModel GetById(int id)
        {
            return _context.TaskResponses.FirstOrDefault(tr => tr.Id == id);
        }

        public void Update(TaskResponseModel taskResponse)
        {
            _context.Entry(taskResponse).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public async Task<IEnumerable<TaskResponseModel>> GetLastTaskResponses()
        {
            // Получаем последние результаты выполнения задачи для каждой задачи
            var lastTaskResponses = await _context.TaskResponses
                .Include(tr => tr.JobResponses)
                .GroupBy(tr => tr.TaskId)
                .Select(group => group.OrderByDescending(tr => tr.Id).First())
                .ToListAsync();

            return lastTaskResponses;
        }
        public async Task<IEnumerable<TaskResponseModel>> GetTaskResponsesByIdAsync(int taskId)
        {
            return await _context.TaskResponses
                                .Where(tr => tr.TaskId == taskId)
                                .Include(tr => tr.JobResponses)
                                .ToListAsync();
        }
        public async Task<(int TotalPages, IEnumerable<TaskResponseModel> TaskResponses)> GetTaskResponsesByIdAndPageAsync(int taskId, int page)
        {
            int pageSize = 4;
            int totalRecords = await _context.TaskResponses
                                     .Where(tr => tr.TaskId == taskId)
                                     .CountAsync();
            List<TaskResponseModel> taskResponses;
            int totalPages = totalRecords / pageSize;
            if (totalRecords % pageSize != 0) {
                totalPages++;
            }
            taskResponses = await _context.TaskResponses
                                      .Where(tr => tr.TaskId == taskId)
                                      .Include(tr => tr.JobResponses)
                                      .OrderByDescending(tr => tr.StartTime)
                                      .Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();
            return (totalPages, taskResponses);
        }

        public async Task<(int TotalPages, IEnumerable<TaskResponseModel> TaskResponses)> GetTaskResponsesByIdAndPageAndSearchDateTimeAsync(int taskId, int page , DateOnly? searchDate)
        {
            int pageSize = 4;
            int totalRecords = await _context.TaskResponses
                                     .Where(tr => tr.TaskId == taskId && DateOnly.FromDateTime(tr.StartTime) == searchDate)
                                     .CountAsync();
            List<TaskResponseModel> taskResponses;
            int totalPages = totalRecords / pageSize;
            if (totalRecords % pageSize != 0)
            {
                totalPages++;
            }
            taskResponses = await _context.TaskResponses
                                      .Where(tr => tr.TaskId == taskId && DateOnly.FromDateTime(tr.StartTime) == searchDate)
                                      .Include(tr => tr.JobResponses)
                                      .OrderByDescending(tr => tr.StartTime)
                                      .Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();
            return (totalPages, taskResponses);
        }
        public async Task<MemoryStream> ArchiveLogsForJobAsync(int jobId, bool deleteLogs)
        {
            var taskResponses = await _context.TaskResponses
                                              .Where(tr => tr.TaskId == jobId)
                                              .Include(tr => tr.JobResponses)
                                              .ToListAsync();

            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var taskResponse in taskResponses)
                {
                    var entry = archive.CreateEntry($"Task_{taskResponse.Id}_Log.txt");
                    using (var entryStream = entry.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.WriteLine($"Task ID: {taskResponse.TaskId}");
                        streamWriter.WriteLine($"Status: {taskResponse.Status}");
                        streamWriter.WriteLine($"Start Time: {taskResponse.StartTime}");
                        streamWriter.WriteLine($"End Time: {taskResponse.EndTime}");
                        streamWriter.WriteLine($"Execution Duration: {taskResponse.ExecutionDuration}");
                        streamWriter.WriteLine("Job Responses:");
                        foreach (var jobResponse in taskResponse.JobResponses)
                        {
                            streamWriter.WriteLine($"\tJob ID: {jobResponse.JobId}");
                            streamWriter.WriteLine($"\tStatus: {jobResponse.Status}");
                            streamWriter.WriteLine($"\tHeader: {jobResponse.Header}");
                            streamWriter.WriteLine($"\tBody: {jobResponse.Body}");
                            streamWriter.WriteLine($"\tExecution Duration: {jobResponse.ExecutionDuration}");
                            streamWriter.WriteLine();
                        }
                        streamWriter.WriteLine();
                    }
                }
            }
            memoryStream.Position = 0;

            if (deleteLogs)
            {
                await DeleteLogsForJobAsync(jobId);
            }

            return memoryStream;
        }

        private async Task DeleteLogsForJobAsync(int jobId)
        {
            var taskResponses = await _context.TaskResponses
                                              .Where(tr => tr.TaskId == jobId)
                                              .Include(tr => tr.JobResponses)
                                              .ToListAsync();

            _context.TaskResponses.RemoveRange(taskResponses);
            await _context.SaveChangesAsync();
        }
    }
}
