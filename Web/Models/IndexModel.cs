using Shcheduler.Core.DAL.DatabaseModels;
using Shcheduler.Core.Dto;

namespace Web.Models
{
    public class IndexModel
    {
        public BigJobDto NewBigJob { get; set; }
        public List<BigJobDto> BigJobs { get; set; }
        public List<AgentModel> agents { get; set; }
        public int ids;
    }
}
