using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.DatabaseModels
{
    public class TaskResponseModel
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        public string Status { get; set; }
        public double ExecutionDuration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<JobResponseModel> JobResponses { get; set; } = new List<JobResponseModel>();
    }

    public class JobResponseModel
    {
        [Key]
        public int Id { get; set; }

        public int? TaskResponseId { get; set; }

        [ForeignKey("TaskResponseId")]
        public TaskResponseModel? TaskResponse { get; set; }
        public string RegExError { get; set; }

        public string? JobId { get; set; }

        //public JobModel Job { get; set; }

        public string Status { get; set; }

        public string? Header { get; set; }

        public string? Body { get; set; }

        public double ExecutionDuration { get; set; }
    }

}
