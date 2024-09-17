using Agent.AgentModels;
using Agent.Interfaces;
using Shcheduler.Core.Dto;
using Shcheduler.Shared;
using System.Collections.Concurrent;
using Shcheduler.Core.DAL.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace Agent.Realizations
{
	public class JobService(IResultService resultService) : IJobService
	{
		public ConcurrentDictionary<int, Schedule> jobdict = new ConcurrentDictionary<int, Schedule>();
		private IResultService _resultService = resultService;
		public void AddOrUpdate(int key, BigJobDto dto)
		{
			var sch = new Schedule()
			{
				BigJobId = dto.BigJobId,
				Cron = dto.Cron,
				JobName = dto.JobName,
				Jobs = dto.Jobs,
				Status = null,
				NextEx = dto.NextEx,
				Timezone = dto.Timezone,
				apiKey = dto.apiKey,
				StoppingByError = dto.StoppingByError
			};
			foreach(var job in sch.Jobs)
			{
				if (job.successRegex == null) job.successRegex = "";
				if (job.errorRegex == null) job.errorRegex = "";
			}
			jobdict.AddOrUpdate(key, sch, (x, y) => sch);
			_resultService.SendStatus(sch);
		}
		public void AddOrUpdate(int key, Schedule dto)
		{
			jobdict.AddOrUpdate(key, dto, (x, y) => dto);
            _resultService.SendStatus(dto);
        }
		public void Remove(int key)
		{
			var sch = new Schedule();
			jobdict.Remove(key, out sch);
		}
		public IEnumerable<Schedule> GetAllJobs()
		{
			var schedules = new List<Schedule>();
			foreach (var task in jobdict.Values)
			{
				schedules.Add(task);
			}
			return schedules;
		}

		public Schedule FindById(int id)
		{
			return jobdict[id];
		}
	}
}
