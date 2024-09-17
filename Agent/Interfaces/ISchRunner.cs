//using Shcheduler.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shcheduler.Core.Dto;
using Agent.AgentModels;
namespace Agent.Interfaces
{
    public interface ISchRunner
	{
		public Task<ResponseInWebDto> Run(Schedule sch);
	}
}
