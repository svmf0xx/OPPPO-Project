using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shcheduler.Core.DAL.DatabaseModels
{
    public class RawResponseModel
    {
        [Key]
        public int ResponseID { get; set; }
        public int TaskID { get; set; }
        public int AgentID { get; set; }
        public string RawData { get; set; }
        public string Status { get; set; }
        public DateTime TimestampUTC { get; set; }
		    public DateTime LastExecutionTime { get; set; }
		    public DateTime NextExecutionTime { get; set; }
	  }
}
