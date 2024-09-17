namespace Shcheduler.Core.Dto
{
    public class JobDto
    {
        public int IdJob { get; set; }
        public string Url { get; set; }
        public string errorRegex { get; set; }
        public string successRegex { get; set; }
    }

}
