using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shcheduler.Core.DAL.DatabaseModels;
using Shcheduler.Core.DAL.Interfaces;
using Shcheduler.Core.Dto;
using Shcheduler.Core.SignalR;
using System.Diagnostics;
using System.Threading.Tasks;
using Web.Dal;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IConfiguration _config;
        private readonly IHubContext<SignalRHub, ISend> _hubContext;
        private readonly ITaskResponseRepository _taskResponseRepository;
        public HomeController(ILogger<HomeController> logger, ITaskRepository taskRepository, IHubContext<SignalRHub, ISend> hubContext, IConfiguration config, IAgentRepository agentRepository, ITaskResponseRepository taskResponseRepository)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _hubContext = hubContext;
            _config = config;
            _agentRepository = agentRepository;
            _taskResponseRepository = taskResponseRepository;
        }
        [HttpPost]
        public async Task<IActionResult> EditJob(IndexModel res)
        {
            var upTask = new TaskModel
            {
                TaskID = res.NewBigJob.BigJobId,
                TaskName = res.NewBigJob.JobName,
                Schedule = res.NewBigJob.Cron,
                ClientTimezone = res.NewBigJob.Timezone,
                Username = this.User.Identities.First().Name,
                ApiKey = res.NewBigJob.apiKey,
                StoppingByError = res.NewBigJob.StoppingByError
            };

            var newJobs = new List<JobModel>();
            foreach (var jobDto in res.NewBigJob.Jobs)
            {
                var newJob = new JobModel
                {
                    TaskID = res.NewBigJob.BigJobId,
                    Url = jobDto.Url,
                    Priority = jobDto.IdJob,
                    errorRegex = jobDto.errorRegex == null ? "" : jobDto.errorRegex,
                    successRegex = jobDto.successRegex == null ? "" : jobDto.successRegex
                };
                newJobs.Add(newJob);
            }
            await _hubContext.Clients.Group(_config.GetSection("SignalRGroupNames:AgentsGroupName").Value).SendModifiedSchedule(res.NewBigJob);
            await _taskRepository.UpdateTaskWithJobsAsync(upTask, newJobs);
            return Redirect("/Home/Index");
        }
        [HttpPost]
        public async Task<IActionResult> DelJob(int idForDel)
        {
            await _hubContext.Clients.Group(_config.GetSection("SignalRGroupNames:AgentsGroupName").Value).SendDeletedSchedule(idForDel);
            Console.WriteLine(idForDel.ToString());
            await _taskRepository.RemoveTaskWithJobsAsync(idForDel);
            return Redirect("/Home/Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddJob(IndexModel res)
        {
            var newTask = new TaskModel
            {
                TaskName = res.NewBigJob.JobName,
                Schedule = res.NewBigJob.Cron,
                ClientTimezone = res.NewBigJob.Timezone,
                ApiKey = res.NewBigJob.apiKey,
                Username = User.Identities.First().Name,
                StoppingByError = res.NewBigJob.StoppingByError
            };

            var newJobs = new List<JobModel>();
            foreach (var jobDto in res.NewBigJob.Jobs)
            {
                var newJob = new JobModel
                {
                    TaskID = 0,
                    Url = jobDto.Url,
                    Priority = jobDto.IdJob,
                    errorRegex = jobDto.errorRegex == null ? "" : jobDto.errorRegex,
                    successRegex = jobDto.successRegex == null ? "" : jobDto.successRegex
                };
                newJobs.Add(newJob);
            }
            var savedTask = await _taskRepository.AddTaskWithJobsAsync(newTask, newJobs);
            res.NewBigJob.BigJobId = savedTask.TaskID;

            await _hubContext.Clients.Group(_config.GetSection("SignalRGroupNames:AgentsGroupName").Value).SendNewSchedule(res.NewBigJob);


            return Redirect("/Home/Index");
        }
        public async Task<IActionResult> DownloadLogArchive(int jobId, bool deleteLogs)
        {
            var archiveStream = await _taskResponseRepository.ArchiveLogsForJobAsync(jobId, deleteLogs);

            // Генерация имени файла архива
            var fileName = $"Job_{jobId}_Logs_{DateTime.Now:yyyyMMddHHmmss}.zip";
            return File(archiveStream, "application/zip", fileName);
        }

        public async Task<IActionResult> Index()
        {
            ViewData["User"] = this.User.Identities.First().Name;
            var username = this.User.Identities.First().Name;
            var tasks = await _taskRepository.GetAllTasksForUserAsync(username); // Используем новый метод
            var bigJobs = new List<BigJobDto>();
            foreach (var task in tasks)
            {
                var jobs = await _taskRepository.GetAllJobsForTaskAsync(task.TaskID);

                var bigJob = new BigJobDto
                {
                    BigJobId = task.TaskID,
                    JobName = task.TaskName,
                    Cron = task.Schedule,
                    Timezone = task.ClientTimezone,
                    apiKey = task.ApiKey,
                    Jobs = jobs.Select(job => new JobDto
                    {
                        IdJob = job.Priority,
                        Url = job.Url,
                        errorRegex = job.errorRegex,
                        successRegex = job.successRegex
                    }).ToList()
                };

                bigJobs.Add(bigJob);
            }

            var model = new IndexModel();

            model.BigJobs = bigJobs;
            var agents = await _agentRepository.GetAllAgentsAsync();

            model.agents = agents.ToList();
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult AboutUs()
        {
            ViewData["User"] = this.User.Identities.First().Name;
            return View();
        }
        public async Task<IActionResult> JobResults()
        {

            var username = this.User.Identities.First().Name;
            ViewData["User"] = username;
            var tasks = await _taskRepository.GetAllTasksForUserAsync(username); // Используем новый метод
            var bigJobs = new List<BigJobDto>();
            foreach (var task in tasks)
            {
                var jobs = await _taskRepository.GetAllJobsForTaskAsync(task.TaskID);

                var bigJob = new BigJobDto
                {
                    BigJobId = task.TaskID,
                    JobName = task.TaskName,
                    Cron = task.Schedule,
                    Timezone = task.ClientTimezone,
                    Jobs = jobs.Select(job => new JobDto
                    {
                        IdJob = job.Priority,
                        Url = job.Url,
                    }).OrderByDescending(job => job.IdJob).ToList()
                };

                bigJobs.Add(bigJob);
            }

            var model = new JobResultModel();
            model.BigJobs = bigJobs;
            //получение данных из бд (нужно передать в эту модель только результат последнего срабатывания)
            var lastTaskResponses = await _taskResponseRepository.GetLastTaskResponses();

            model.Responses = lastTaskResponses.Select(tr => new ResponseInWebDto
            {
                BigJobID = tr.TaskId,
                Status = tr.Status,
                ExecutionDuration = tr.ExecutionDuration,
                StartTime = tr.StartTime,
                EndTime = tr.EndTime,
                ResponsesList = tr.JobResponses.Select(jr => new ResponseDataDto
                {
                    JobID = jr.JobId,
                    Status = jr.Status,
                    Header = jr.Header,
                    Body = jr.Body,
                    ExecutionDuration = jr.ExecutionDuration
                }).ToList()
            }).ToList();
            return View(model);
        }
        public async Task<IActionResult> FullLogs(int jobId, int page = 1, DateOnly? searchDate = null)
        {
            ViewData["User"] = this.User.Identities.First().Name;
            var model = new FullLogsModel();
            if (searchDate == null)
            {
                var (totalPages, TaskResponses) = await _taskResponseRepository.GetTaskResponsesByIdAndPageAsync(jobId, page);
                var task = await _taskRepository.GetTaskByIdAsync(jobId);
                model.Responses = TaskResponses.Select(tr => new ResponseInWebDto
                {
                    BigJobID = tr.TaskId,
                    Status = tr.Status,
                    ExecutionDuration = tr.ExecutionDuration,
                    StartTime = tr.StartTime,
                    EndTime = tr.EndTime,
                    ResponsesList = tr.JobResponses.Select(jr => new ResponseDataDto
                    {
                        JobID = jr.JobId,
                        Status = jr.Status,
                        Header = jr.Header,
                        Body = jr.Body,
                        ExecutionDuration = jr.ExecutionDuration
                    }).ToList()
                }).ToList();
                var jobs = await _taskRepository.GetAllJobsForTaskAsync(task.TaskID);
                var bigJob = new BigJobDto
                {
                    BigJobId = task.TaskID,
                    JobName = task.TaskName,
                    Cron = task.Schedule,
                    Timezone = task.ClientTimezone,
                    apiKey = task.ApiKey,
                    Jobs = jobs.Select(job => new JobDto
                    {
                        IdJob = job.Priority,
                        Url = job.Url,
                    }).ToList()
                };

                model.BigJob = bigJob;
                model.pageCount = totalPages;
                model.selectedPage = page;
            }
            else
            {
                var (totalPages, TaskResponses) = await _taskResponseRepository.GetTaskResponsesByIdAndPageAndSearchDateTimeAsync(jobId, page, searchDate);
                var task = await _taskRepository.GetTaskByIdAsync(jobId);
                model.Responses = TaskResponses.Select(tr => new ResponseInWebDto
                {
                    BigJobID = tr.TaskId,
                    Status = tr.Status,
                    ExecutionDuration = tr.ExecutionDuration,
                    StartTime = tr.StartTime,
                    EndTime = tr.EndTime,
                    ResponsesList = tr.JobResponses.Select(jr => new ResponseDataDto
                    {
                        Status = jr.Status,
                        Header = jr.Header,
                        Body = jr.Body,
                        ExecutionDuration = jr.ExecutionDuration
                    }).ToList()
                }).ToList();
                var jobs = await _taskRepository.GetAllJobsForTaskAsync(task.TaskID);
                var bigJob = new BigJobDto
                {
                    BigJobId = task.TaskID,
                    JobName = task.TaskName,
                    Cron = task.Schedule,
                    Timezone = task.ClientTimezone,
                    apiKey = task.ApiKey,
                    Jobs = jobs.Select(job => new JobDto
                    {
                        IdJob = job.Priority,
                        Url = job.Url,
                    }).ToList()
                };

                model.BigJob = bigJob;
                model.pageCount = totalPages;
                model.selectedPage = page;
                model.chooseDate = searchDate;
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public string TestRegEx(Random random)
        {
            var number = random.Next(1, 100);
            return number > 30 ? "Success OK" : "Error Fail Trouble";
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult TestStandart(Random random)
        {
            var number = random.Next(1, 100);
            return number > 30 ? Ok() : BadRequest();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
