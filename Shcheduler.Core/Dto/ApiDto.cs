using Shcheduler.Core.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shcheduler.Core.Dto
{
    public class ApiJobDto
    {
        public int IdJob { get; set; }
        [ValidUrl]
        public string Url { get; set; }
        public string errorRegex { get; set; } // Добавлено поле errorRegex
        public string successRegex { get; set; } // Добавлено поле successRegex
    }

    public class ApiNewBigJobDto
    {
        public int BigJobId { get; set; }
        public string JobName { get; set; }
        [CronExpression]
        public string Cron { get; set; }
        public string Timezone { get; set; }
        [ValidApiKey]
        public string ApiKey { get; set; }
        public bool StoppingByError { get; set; } // Добавлено поле StoppingByError
        public List<ApiJobDto> Jobs { get; set; }
    }
}
