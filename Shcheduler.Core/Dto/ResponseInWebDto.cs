using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.Dto
{
    public class ResponseInWebDto
    {
        public int BigJobID { get; set; }
        public string Status { get; set; }
        public List<ResponseDataDto> ResponsesList { get; set; } = new List<ResponseDataDto>();
        public double ExecutionDuration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class ResponseDataDto
    {
        public string JobID { get; set; }
        public string Status { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string RegExError { get; set; } = "";
        public double ExecutionDuration { get; set; }
        public DateTime ExecutionTime { get; set; }
    }
}
