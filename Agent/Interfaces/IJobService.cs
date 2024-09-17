using Agent.AgentModels;
using Shcheduler.Core.Dto;
using System.Collections.Concurrent;

namespace Agent.Interfaces
{
	public interface IJobService
	{

		public void AddOrUpdate(int key, BigJobDto job);
		public void AddOrUpdate(int key, Schedule sch);
		public IEnumerable<Schedule> GetAllJobs();
		public void Remove(int key);
		public Schedule FindById(int id);
	}
}
