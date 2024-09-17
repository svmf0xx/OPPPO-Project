using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.DAL.DatabaseModels
{
    public class JobModel
    {
        [Key]
        public int JobID { get; set; }
        public int TaskID { get; set; }
        public string Url { get; set; }
        public TaskModel Task { get; set; }
        public int Status { get; set; } //
        public int Priority { get; set; } // для замапливания с дто нужно IdJob
        public string errorRegex { get; set; }
        public string successRegex { get; set; }
    }
}
