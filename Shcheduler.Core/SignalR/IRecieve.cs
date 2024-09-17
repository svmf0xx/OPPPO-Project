using Shcheduler.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.SignalR
{
	public interface IRecieve
	{
		public Task RecieveExecutionTime(JobResultDto sch);
		public Task RecieveResponse(ResponseInWebDto resp);
		public Task RecieveTimeTable(TimeTableDto timetable);
	}
}
