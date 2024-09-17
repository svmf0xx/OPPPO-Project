using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shcheduler.Core.DAL.DatabaseModels
{
    public class TaskModel
    {
        [Key]
        public int TaskID { get; set; }
        public string ApiKey { get; set; }
        public string TaskName { get; set; }
        public string Schedule { get; set; } //cron
        public bool StoppingByError { get; set; }
        //public string? Parameters { get; set; }
        public string? Status { get; set; } //running done pause
        public ICollection<JobModel> Jobs { get; set; }
        //public int? AgentID { get; set; } если будет много агентов
        public DateTime TimestampUTC { get; set; } = DateTime.UtcNow;
        public string ClientTimezone { get; set; } // сделать обязатиельным когда я буду возвращать таймзону с веба и поменять тип данных
        public string Username { get; set; }
    }
}
