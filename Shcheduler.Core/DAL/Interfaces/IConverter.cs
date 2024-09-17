using Shcheduler.Core.DAL.DatabaseModels;
using Shcheduler.Core.Dto;

namespace Shcheduler.Core.DAL.Interfaces
{
	public interface IConverter
	{
		public AgentModel Convert(AgentAppSettings model);
		public IEnumerable<JobDto> ConvertJobs(IEnumerable<JobModel> jobs);
		public IEnumerable<BigJobDto> Convert(IEnumerable<TaskModel> model);
	}
}