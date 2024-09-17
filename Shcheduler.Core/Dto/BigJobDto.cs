namespace Shcheduler.Core.Dto
{
    public class BigJobDto
    {
        public int BigJobId { get; set; }
        public string apiKey { get; set; }
        public string JobName { get; set; }
        public List<JobDto> Jobs { get; set; }
        public string Cron { get; set; }
        public bool StoppingByError { get; set; }
        public string Timezone { get; set; }
        public string Status { get; set; }
        public string RegEx { get; set; } //если строка пустая то ничего делать не надо
        // присылаешь статус Статус - ошибка по регексу
		public DateTime LastEx { get; set; }
		public DateTime NextEx { get; set; }
	}

    public class AgentInitDto
    {
        public int AgentId { get; set; }
        public string ApiKey { get; set; }
        public List<BigJobDto> Schedule { get; set; }
    }

}
