using Shcheduler.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.SignalR
{
    public interface ISend
	{
		public Task RunJobNow(int jobId);
		public Task SendJobStatus(BigJobDto bigJob);
		public Task SendNewSchedule(BigJobDto newSch);
		public Task SendModifiedSchedule(BigJobDto newResource);
		public Task SendDeletedSchedule(int idForDel);
		public Task SendExecutionTime(JobResultDto result);
		public Task CheckStatus(int id);
		public Task SendResponse(ResponseInWebDto resp);
		public Task ExecuteNow (int id);
		public Task UpdateAgentsForClient(string apiKey);
		public Task ExecTimeTable(int id);
        public Task SendTimeTableToClient(TimeTableDto timetable);
        public Task SendApiSchedule(ApiNewBigJobDto newBigJobDto);
        public Task SendApiModifiedSchedule(ApiNewBigJobDto newBigJobDto);
    }
}
