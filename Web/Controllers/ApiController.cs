using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shcheduler.Core.DAL.DatabaseModels;
using Shcheduler.Core.DAL.Interfaces;
using Shcheduler.Core.SignalR;
using Shcheduler.Core.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IHubContext<SignalRHub, ISend> _hubContext;
        private readonly IConfiguration _config;

        public ApiController(ITaskRepository taskRepository, IHubContext<SignalRHub, ISend> hubContext, IConfiguration config)
        {
            _taskRepository = taskRepository;
            _hubContext = hubContext;
            _config = config;
        }

        /// <summary>
        /// Добавление нового cron-задания.
        /// </summary>
        /// <param name="newBigJobDto">DTO для нового задания.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/Api/AddJob
        ///     {
        ///         "JobName": "Test",
        ///         "Cron": "* * * * *",
        ///         "Timezone": "UTC",
        ///         "ApiKey": "123",
        ///         "StoppingByError": true,
        ///         "Jobs": [
        ///             {
        ///                 "Url": "https://localhost:7156/",
        ///                 "IdJob": 0,
        ///                 "errorRegex": "error-pattern",
        ///                 "successRegex": "success-pattern"
        ///             },
        ///             {
        ///                 "Url": "https://localhost:7156/",
        ///                 "IdJob": 1,
        ///                 "errorRegex": "error-pattern",
        ///                 "successRegex": "success-pattern"
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        [HttpPost]
        [Route("AddJob")]
        public async Task<IActionResult> AddJob(ApiNewBigJobDto newBigJobDto)
        {
            var newTask = new TaskModel
            {
                TaskName = newBigJobDto.JobName,
                Schedule = newBigJobDto.Cron,
                ClientTimezone = newBigJobDto.Timezone,
                ApiKey = newBigJobDto.ApiKey,
                Username = User.Identity.Name,
                StoppingByError = newBigJobDto.StoppingByError
            };

            var newJobs = new List<JobModel>();
            foreach (var jobDto in newBigJobDto.Jobs)
            {
                var newJob = new JobModel
                {
                    TaskID = 0, // TaskID будет установлен при сохранении TaskModel
                    Url = jobDto.Url,
                    Priority = jobDto.IdJob,
                    errorRegex = jobDto.errorRegex ?? "", // Добавляем обработку errorRegex
                    successRegex = jobDto.successRegex ?? "" // Добавляем обработку successRegex
                };
                newJobs.Add(newJob);
            }

            var savedTask = await _taskRepository.AddTaskWithJobsAsync(newTask, newJobs);
            newBigJobDto.JobName = savedTask.TaskName; // Обновите JobName с TaskName
            newBigJobDto.BigJobId = savedTask.TaskID;
            await _hubContext.Clients.Group(_config.GetSection("SignalRGroupNames:AgentsGroupName").Value).SendApiSchedule(newBigJobDto);

            return CreatedAtAction(nameof(AddJob), new { id = savedTask.TaskID }, newBigJobDto);
        }

        /// <summary>
        /// Редактирует существующее задание.
        /// </summary>
        /// <param name="newBigJobDto">DTO для редактирования задания.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     PUT /api/Api/EditJob
        ///     {
        ///         "BigJobId": 1,
        ///         "JobName": "EditTest",
        ///         "Cron": "* * * * *",
        ///         "Timezone": "UTC",
        ///         "ApiKey": "123",
        ///         "StoppingByError": true,
        ///         "Jobs": [
        ///             {
        ///                 "Url": "https://localhost:7156/",
        ///                 "IdJob": 0,
        ///                 "errorRegex": "error-pattern",
        ///                 "successRegex": "success-pattern"
        ///             },
        ///             {
        ///                 "Url": "https://localhost:7156/",
        ///                 "IdJob": 1,
        ///                 "errorRegex": "error-pattern",
        ///                 "successRegex": "success-pattern"
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        [HttpPut]
        [Route("EditJob")]
        public async Task<IActionResult> EditJob(ApiNewBigJobDto newBigJobDto)
        {
            var upTask = new TaskModel
            {
                TaskID = newBigJobDto.BigJobId,
                TaskName = newBigJobDto.JobName,
                Schedule = newBigJobDto.Cron,
                ClientTimezone = newBigJobDto.Timezone,
                Username = User.Identity.Name,
                ApiKey = newBigJobDto.ApiKey,
                StoppingByError = newBigJobDto.StoppingByError
            };

            var updatedJobs = new List<JobModel>();
            foreach (var jobDto in newBigJobDto.Jobs)
            {
                var updatedJob = new JobModel
                {
                    TaskID = upTask.TaskID,
                    Url = jobDto.Url,
                    Priority = jobDto.IdJob,
                    errorRegex = jobDto.errorRegex ?? "", // Добавлено поле errorRegex
                    successRegex = jobDto.successRegex ?? "" // Добавлено поле successRegex
                };
                updatedJobs.Add(updatedJob);
            }


            await _taskRepository.UpdateTaskWithJobsAsync(upTask, updatedJobs);

            await _hubContext.Clients.Group(_config.GetSection("SignalRGroupNames:AgentsGroupName").Value).SendApiModifiedSchedule(newBigJobDto);

            return Ok(newBigJobDto);
        }
    }
}