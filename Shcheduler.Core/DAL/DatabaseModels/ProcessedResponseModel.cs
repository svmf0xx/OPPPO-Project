using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shcheduler.Core.DAL.DatabaseModels
{
    public class ProcessedResponseModel
    {
        [Key]
        public int ProcessedResponseID { get; set; }
        public int ResponseID { get; set; }
        public string ProcessedData { get; set; }
        public string Status { get; set; }
        public DateTime TimestampUTC { get; set; }
    }

}
