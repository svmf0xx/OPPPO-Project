using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.Dto
{
    public class JobResultDto
    {
        public int SchId { get; set; }
        //public ConcurrentDictionary<int, int> StatusDict { get; set; } = new ConcurrentDictionary<int, int>();
        public string Status { get; set; }
        public DateTime LastEx {  get; set; }
		    public DateTime NextEx { get; set; }
	  }
}
