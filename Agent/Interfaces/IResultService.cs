using Agent.AgentModels;
using Shcheduler.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Interfaces
{
    public interface IResultService
	{
		public void SendStatus(Schedule sch);
        public void SendStatus(BigJobDto dto);
        public Schedule UpdateExectutionTime(Schedule sch, DateTime NextEx);
	}
}
