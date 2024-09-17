using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.Dto
{
    public class TimeTableDto
    {
        public int BigJobId { get; set; }
        public List<DateTime> DateList { get; set; }
    }
}
