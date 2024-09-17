using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shcheduler.Core.DAL.DatabaseModels;
using Shcheduler.Core.DAL.Interfaces;
using Shcheduler.Core.Dto;

namespace Shcheduler.Core.DAL
{
    public class Converter() : IConverter
	{
		public IEnumerable<BigJobDto> Convert(IEnumerable<TaskModel> model) 
		{
			foreach (var item in model)
			{
				yield return new BigJobDto()
				{
					BigJobId = item.TaskID,
					Cron = item.Schedule,
					JobName = item.TaskName,
					apiKey = item.ApiKey,
					StoppingByError = item.StoppingByError
					//Jobs = Convert(item.Jobs).ToList()
					//Timezone = item.ClientTimezone
				};
			}

		}

		public IEnumerable<JobDto> ConvertJobs(IEnumerable<JobModel> jobs)
		{
			
			foreach (var job in jobs)
			{
				yield return new JobDto()
				{
					IdJob = job.JobID,
					Url = job.Url,
					successRegex = job.successRegex,
					errorRegex = job.errorRegex
				};
			}
			
		}

		public AgentModel Convert (AgentAppSettings model)
		{
			return new AgentModel()
			{
				AgentName = model.AgentName,
				Location = model.AgentLocation,
				ApiKey = model.ApiKey
			};
		}
	}
}
