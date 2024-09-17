using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.DatabaseModels
{
    public class AgentModel
    {
        [Key]
        public int AgentID { get; set; }
        public string AgentName { get; set; }
        public string Location { get; set; }
        public string ApiKey { get; set; }
        //public DateTime LastHeartbeatUTC { get; set; } на будущее для проверки работоспособности
    }

}
