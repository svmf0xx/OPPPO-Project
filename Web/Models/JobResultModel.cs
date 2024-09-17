using Shcheduler.Core.Dto;

namespace Web.Models
{
    public class JobResultModel
    {
        public List<BigJobDto> BigJobs { get; set; }
        public List<ResponseInWebDto> Responses { get; set; }

    }
}
