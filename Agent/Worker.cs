using Agent.Realizations;
using Shcheduler.Core;
using Agent.Interfaces;
using System.Threading;
using System;
using Cronos;
using Shcheduler.Core.DAL;
using Shcheduler.Core.Dto;
using Microsoft.AspNetCore.SignalR.Client;
using Shcheduler.Core.SignalR;
using Agent.AgentModels;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using static System.Net.Mime.MediaTypeNames;
namespace Agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AgentAppSettings _appSettings;
        private readonly IScheduleService _schService;
        private readonly IJobService _jobService;
        private readonly IResultService _resService;
        private readonly ISchRunner _schRunner;
        private readonly TimeZoneInfo _timeZone;
        private readonly ISchStatusService _statusService;
        private readonly ShchedulerContext _dbContext;
        private readonly SignalRAgent _signalRAgent;
        private readonly AgentAppSettings _agent;
        private readonly HubConnection _hubConnection;
        private string apiKey;
        private List<Schedule> schToRun;
        private bool IsStatusesSended = false;

        public Worker(HubConnection hubConnection, AgentAppSettings agent, SignalRAgent signalRAgent, ISchStatusService statusService, ILogger<Worker> logger, AgentAppSettings appSettings, IScheduleService schService, IJobService jobService, IResultService resService, ISchRunner schRunner)
        {
            _hubConnection = hubConnection;
            _logger = logger;
            _appSettings = appSettings;
            _schService = schService;
            _jobService = jobService;
            _resService = resService;
            _schRunner = schRunner;
            _timeZone = TimeZoneInfo.Local;
            _statusService = statusService;
            _signalRAgent = signalRAgent;
            _agent = agent;

        }
        private async Task ConnectWithRetryAsync(CancellationToken stoppingToken)  //инициализация, получение всех расписаний
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var init = await _signalRAgent.StartConnectionAsync(_agent);

                    Console.WriteLine($"//Connected, my apikey = {init.ApiKey}//\n");
                    apiKey = init.ApiKey;
                    foreach (var item in init.Schedule)
                    {
                        if (item.apiKey == apiKey)
                        {
                            CronExpression expression;
                            try
                            {
                                expression = CronExpression.Parse(item.Cron);
                            }
                            catch (Exception ex)
                            {
                                item.Cron = "0 * * * *";
                                expression = CronExpression.Parse(item.Cron);
                            }
                            var next = expression.GetNextOccurrence(DateTimeOffset.Now, _timeZone).Value.DateTime;
                            item.NextEx = next;
                            _jobService.AddOrUpdate(item.BigJobId, item);
                            IsStatusesSended = false;
                        }
                    }
                    IsStatusesSended = false;
                    return;
                }
                catch (Exception ex)
                {
                    await Task.Delay(100);
                }
                finally { stoppingToken.ThrowIfCancellationRequested(); }
            }
        }
        private async void Init(CancellationToken stoppingToken) //инициализация сокетов
        {
            try
            {
                await ConnectWithRetryAsync(stoppingToken);
                IsStatusesSended = false;

                _hubConnection.On<BigJobDto>(nameof(ISend.SendNewSchedule), res =>
                {
                    if (res.apiKey == apiKey)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("//New Resource recieved: ");
                        Console.ResetColor();
                        Console.Write($"{res.JobName} {res.Cron} ");
                        foreach (var job in res.Jobs)
                        {
                            Console.Write($"{job.Url} ");
                        }
                        Console.Write("//");
                        Console.WriteLine("\n");
                        foreach (var job in res.Jobs)
                        {
                            if (job.errorRegex == null) job.errorRegex = "";
                            if (job.successRegex == null) job.successRegex = "";
                        }
                        CronExpression expression;
                        try
                        {
                            expression = CronExpression.Parse(res.Cron);
                        }
                        catch (Exception ex)
                        {
                            res.Cron = "0 * * * *";
                            expression = CronExpression.Parse(res.Cron);
                        }
                        var next = expression.GetNextOccurrence(DateTimeOffset.Now, _timeZone).Value.DateTime;
                        res.NextEx = next;
                        _jobService.AddOrUpdate(res.BigJobId, res);
                        IsStatusesSended = false;
                    }

                });
                _hubConnection.On<BigJobDto>(nameof(ISend.SendApiSchedule), res =>
                {
                    if (res.apiKey == apiKey)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("//New Resource recieved: ");
                        Console.ResetColor();
                        Console.Write($"{res.JobName} {res.Cron} ");
                        foreach (var job in res.Jobs)
                        {
                            Console.Write($"{job.Url} ");
                        }
                        Console.Write("//");
                        Console.WriteLine("\n");
                        foreach (var job in res.Jobs)
                        {
                            if (job.errorRegex == null) job.errorRegex = "";
                            if (job.successRegex == null) job.successRegex = "";
                        }
                        CronExpression expression;
                        try
                        {
                            expression = CronExpression.Parse(res.Cron);
                        }
                        catch (Exception ex)
                        {
                            res.Cron = "0 * * * *";
                            expression = CronExpression.Parse(res.Cron);
                        }
                        var next = expression.GetNextOccurrence(DateTimeOffset.Now, _timeZone).Value.DateTime;
                        res.NextEx = next;
                        _jobService.AddOrUpdate(res.BigJobId, res);
                        IsStatusesSended = false;
                    }
                });
                _hubConnection.On<BigJobDto>(nameof(ISend.SendModifiedSchedule), res =>
                {
                    Console.Write("//Modified: ");
                    Console.Write($"{res.JobName} {res.Cron} ");
                    foreach (var job in res.Jobs)
                    {
                        Console.Write($"{job.Url} ");
                    }
                    Console.Write("//");
                    Console.WriteLine("");
                    CronExpression expression;
                    try
                    {
                        expression = CronExpression.Parse(res.Cron);
                    }
                    catch (Exception ex)
                    {
                        res.Cron = "0 * * * *";
                        expression = CronExpression.Parse(res.Cron);
                    }
                    var next = expression.GetNextOccurrence(DateTimeOffset.Now, _timeZone).Value.DateTime;
                    res.NextEx = next;
                    try
                    {
                        var sch = _jobService.FindById(res.BigJobId);
                        if (res.apiKey == sch.apiKey)
                            _jobService.AddOrUpdate(res.BigJobId, res);

                        else
                            _jobService.Remove(res.BigJobId);
                    }
                    catch {
                        if (res.apiKey == apiKey)
                            _jobService.AddOrUpdate(res.BigJobId, res);
                    }
                });
                _hubConnection.On<BigJobDto>(nameof(ISend.SendApiModifiedSchedule), res =>
                {
                    if (res.apiKey == apiKey)
                    {
                        Console.Write("//Modified: ");
                        Console.Write($"{res.JobName} {res.Cron} ");
                        foreach (var job in res.Jobs)
                        {
                            Console.Write($"{job.Url} ");
                        }
                        Console.Write("//");
                        Console.WriteLine("");
                        CronExpression expression;
                        try
                        {
                            expression = CronExpression.Parse(res.Cron);
                        }
                        catch (Exception ex)
                        {
                            res.Cron = "0 * * * *";
                            expression = CronExpression.Parse(res.Cron);
                        }
                        var next = expression.GetNextOccurrence(DateTimeOffset.Now, _timeZone).Value.DateTime;
                        res.NextEx = next;
                        _jobService.AddOrUpdate(res.BigJobId, res);
                        IsStatusesSended = false;
                    }
                });
                _hubConnection.On<int>(nameof(ISend.SendDeletedSchedule), id =>
                {
                    Console.Write("//");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Deleted");
                    Console.ResetColor();
                    Console.Write($" schedule with id {id}");
                    Console.Write("//");
                    Console.WriteLine("\n");
                    try
                    {
                        _jobService.Remove(id);
                    }
                    catch { }
                });
                _hubConnection.On<int>(nameof(ISend.CheckStatus), id =>
                {
                    _signalRAgent.UpdExTime(_jobService.FindById(id));
                    schToRun = _jobService.GetAllJobs().ToList();

                    foreach (var sch in schToRun)
                    {
                        _resService.SendStatus(sch);
                    }

                });
                _hubConnection.On<int>(nameof(ISend.ExecuteNow), id =>
                {
                    Console.Write($"//Executed Schedule with id = {id}");
                    Console.Write("//");
                    Console.WriteLine("");
                    try
                    {
                        var sch = _jobService.FindById(id);
                        sch.LastEx = DateTime.Now;
                        sch.Status = _schRunner.Run(sch).Result.Status;
                        _resService.SendStatus(sch);
                    }
                    catch (Exception ex) { }
                });
                _hubConnection.On<int>(nameof(ISend.ExecTimeTable), id =>
                {
                    Console.Write("//");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("TimeTable");
                    Console.ResetColor();
                    Console.Write($" is sended for Shedule with id {id}");
                    Console.Write("//");
                    Console.WriteLine("\n");
                    try
                    {
                        var sch = _jobService.FindById(id);
                        var next = DateTimeOffset.Now;
                        var TimeTable = new TimeTableDto();
                        TimeTable.BigJobId = id;
                        TimeTable.DateList = new List<DateTime>();
                        for (int i = 0; i < 10; i++)
                        {
                            var expression = CronExpression.Parse(sch.Cron);
                            next = expression.GetNextOccurrence(next, _timeZone).Value;
                            TimeTable.DateList.Add(next.DateTime);
                        }
                        _signalRAgent.SendTimeTable(TimeTable).Wait();
                    }
                    catch (Exception ex) { }

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ExecuteAsync error in Agent ({ex.Message})");
            }

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Init(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    schToRun = _jobService.GetAllJobs().ToList();
                    foreach (var sch in schToRun)
                    {
                        _resService.SendStatus(sch);
                    }

                    foreach (var sch in schToRun)
                    {

                        if (DateTime.Compare(sch.NextEx, DateTime.Now) <= 0 && DateTime.Compare(sch.NextEx, DateTime.MinValue) == 1) // пришло ли время?
                        {
                            CronExpression expression;
                            try
                            {
                                expression = CronExpression.Parse(sch.Cron);
                            }
                            catch
                            {
                                sch.Cron = "0 * * * *";
                                expression = CronExpression.Parse(sch.Cron);
                            }

                            var nextEx = expression.GetNextOccurrence(DateTimeOffset.Now.AddSeconds(5), _timeZone).Value.DateTime;
                            _resService.UpdateExectutionTime(sch, nextEx);

                            var jobTask = _schRunner.Run(sch); //отправляется клиенту здесь же
                            sch.Status = jobTask.Result.Status;
                            _jobService.AddOrUpdate(sch.BigJobId, sch);
                            _resService.SendStatus(sch);


                            if (!_statusService.GetCurrTasks().Contains(jobTask))
                            {

                                _statusService.AddToRunning(jobTask);
                            }
                        }
                    }
                }
                catch (Exception ex) { }
                finally { await Task.Delay(TimeSpan.FromSeconds(_appSettings.UpdateSchTime), stoppingToken); }
            }
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping...");
            await Task.Run(() =>
            {
                _signalRAgent.StopAgent(apiKey);
            }, stoppingToken);


            await base.StopAsync(stoppingToken);
        }
    }
}
