using Shcheduler.Core.DAL.DatabaseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskModel>> GetAllTasksAsync();
        Task<TaskModel> GetTaskByIdAsync(int taskId);
        Task AddTaskAsync(TaskModel task);
        Task UpdateTaskAsync(TaskModel task);
        Task RemoveTaskAsync(int taskId);
        Task<TaskModel> AddTaskWithJobsAsync(TaskModel task, IEnumerable<JobModel> jobs);
        Task<IEnumerable<JobModel>> GetAllJobsForTaskAsync(int taskId);
        Task RemoveTaskWithJobsAsync(int taskId);
        Task<TaskModel> UpdateTaskWithJobsAsync(TaskModel updatedTask, IEnumerable<JobModel> updatedJobs);
        Task<IEnumerable<TaskModel>> GetAllTasksForUserAsync(string username);
    }
}
