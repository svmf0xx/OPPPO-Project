using Agent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Shcheduler.Shared;
using Shcheduler.Core.Dto;
using Agent.AgentModels;
namespace Agent.Realizations
{
    public class SchResultService(ILogger<SchResultService> logger, SignalRAgent signalR): IResultService
	{
		private readonly ISignalRAgent _signalR = signalR;
		private readonly ILogger<SchResultService> _logger = logger;
		public void SendStatus(Schedule sch)
		{
			try
			{ 
				_signalR.UpdExTime(sch);
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, $"Send error ({ex.GetType().Name}: {ex.Message})");
			}
		}
        public void SendStatus(BigJobDto dto)
        {
            try
            {
				var sch = new Schedule()
				{
					BigJobId = dto.BigJobId,
					Cron = dto.Cron,
					Status = dto.Status,
					JobName = dto.JobName,
					Jobs = dto.Jobs,
					LastEx = dto.LastEx,
					NextEx = dto.NextEx,
					Timezone = dto.Timezone,
				};
                _signalR.UpdExTime(sch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Send error ({ex.GetType().Name}: {ex.Message})");
            }
        }
        public void SendResponce(Schedule sch) 
		{
			
		}
		public Schedule UpdateExectutionTime(Schedule sch, DateTime NextEx)
		{
			if (sch.NextEx == DateTime.MinValue)
			{
				sch.NextEx = NextEx;
			}
			else
			{
				sch.LastEx = sch.NextEx;
				sch.NextEx = NextEx;
			}
			_signalR.UpdExTime(sch);
			return sch;
		}

	}
}
