using Shcheduler.Core.Dto;

namespace Web.Models
{
    public class FullLogsModel
    {
        public int pageCount { get; set; }
        public int selectedPage { get; set; }
        public DateOnly? chooseDate { get; set; }
        public BigJobDto BigJob { get; set; }
        public List<ResponseInWebDto> Responses { get; set; }

    }
}
