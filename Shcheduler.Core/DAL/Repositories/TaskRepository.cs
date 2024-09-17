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
    public class TaskRepository:ITaskRepository
    {
        private readonly ShchedulerContext _context;
        public TaskRepository(ShchedulerContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TaskModel>> GetAllTasksAsync()
        {
            var tasks = await _context.Tasks.ToListAsync();
            foreach(var task in tasks)
            {
                task.Jobs = await _context.Jobs.Where(job=>job.TaskID==task.TaskID).ToListAsync();
            }
            return tasks;
        }

        public async Task<TaskModel> GetTaskByIdAsync(int taskId)
        {
            return await _context.Tasks.FindAsync(taskId);
        }

        public async Task AddTaskAsync(TaskModel task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TaskModel task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveTaskWithJobsAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                var jobsToRemove = await _context.Jobs.Where(job => job.TaskID == taskId).ToListAsync();
                _context.Jobs.RemoveRange(jobsToRemove);
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<TaskModel> AddTaskWithJobsAsync(TaskModel task, IEnumerable<JobModel> jobs)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            var taskId = task.TaskID;
            foreach (var job in jobs)
            {
                job.TaskID = taskId;
                _context.Jobs.Add(job);
            }

            await _context.SaveChangesAsync();

            return task;
        }
        public async Task<IEnumerable<JobModel>> GetAllJobsForTaskAsync(int taskId)
        {
            return await _context.Jobs.Where(job => job.TaskID == taskId).ToListAsync();
        }
        public async Task<TaskModel> UpdateTaskWithJobsAsync(TaskModel updatedTask, IEnumerable<JobModel> updatedJobs)
        {
            _context.Tasks.Update(updatedTask);
            var existingJobs = await GetAllJobsForTaskAsync(updatedTask.TaskID);
            _context.Jobs.RemoveRange(existingJobs);
            foreach (var job in updatedJobs)
            {
                _context.Jobs.Add(job);
            }
            await _context.SaveChangesAsync();

            return updatedTask;
        }
        public async Task<IEnumerable<TaskModel>> GetAllTasksForUserAsync(string username)
        {
            var tasks = await _context.Tasks
                .Where(task => task.Username == username)
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.Jobs = await _context.Jobs.Where(job => job.TaskID == task.TaskID).ToListAsync();
            }

            return tasks;
        }

    }
}
