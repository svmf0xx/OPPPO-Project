using Agent.Interfaces;
using System.Net;
using System.Diagnostics;
using Shcheduler.Core.Dto;
using Agent.AgentModels;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;
namespace Agent.Realizations
{
    public class SchRunner(ISignalRAgent signalRAgent, HttpClient httpClient, ILogger<SchRunner> logger) : ISchRunner
    {
        HttpClient _httpClient = httpClient;
        //ILogger<SchRunner> _logger = logger;
        ISignalRAgent _signalRAgent = signalRAgent;

        public async Task<ResponseInWebDto> Run(Schedule sch)
        {
            Console.WriteLine($"||Runned Schedule #{sch.BigJobId} at {DateTime.Now.ToString("HH:mm dd-MM-yyyy")}");
            var timeAll = Stopwatch.StartNew();
            var result = new ResponseInWebDto() { BigJobID = sch.BigJobId, StartTime = DateTime.Now.AddMilliseconds(0) };
            var _stopByErr = false;
            string regExError = "Job Error";

            foreach (var job in sch.Jobs)
            {
                string status;
                string Body;
                string Header;
                ResponseDataDto jobresp;
                var timeJob = Stopwatch.StartNew();
                var response = new HttpResponseMessage();
                if (_stopByErr)
                {
                    jobresp = new ResponseDataDto()
                    {
                        JobID = job.Url,
                        Body = "Bad Response",
                        Header = "Bad Response",
                        Status = "Stopped by Previous Job",
                        ExecutionDuration = 0,
                        RegExError = "Previous job is Error"
                    };
                    result.ResponsesList.Add(jobresp);
                    continue;
                }

                try
                {
                    response = await _httpClient.GetAsync(job.Url);
                    Body = await response.Content.ReadAsStringAsync();
                    Header = response.Headers.ToString();
                }
                catch (Exception ex)
                {
                    Body = "Error";
                    Header = "Error";

                    timeJob.Stop();
                    jobresp = new ResponseDataDto()
                    {
                        JobID = job.Url,
                        Body = "Bad Response",
                        Header = "Bad Response",
                        Status = "Something BAD",
                        ExecutionDuration = timeJob.ElapsedMilliseconds,
                        RegExError = regExError
                    };
                    result.Status = "Bad Response";
                    result.ResponsesList.Add(jobresp);
                    if (sch.StoppingByError) _stopByErr = true;
                    continue;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    bool headercheck = false;
                    bool bodycheck = false;
                    try
                    {
                        headercheck = CheckRegEx(job.successRegex, job.errorRegex, Header);
                        bodycheck = CheckRegEx(job.successRegex, job.errorRegex, Body);
                    }
                    catch(Exception ex) { }
                    bool check = headercheck && bodycheck;

                    regExError = !headercheck && !bodycheck ? "Both" :
                                        !headercheck ? "Header" :
                                        !bodycheck ? "Body" :
                                        "No Errors";

                    timeJob.Stop();
                    status = check ? "OK" : "RegEx detected Error";
                    if (!check) result.Status = "RegEx Error";
                }
                else
                {
                    timeJob.Stop();
                    status = "Bad Response";
                    result.Status = "Bad Response";
                    Body = "Error";
                }

                jobresp = new ResponseDataDto()
                {
                    JobID = job.Url,
                    Body = Body,
                    Header = Header,
                    Status = status,
                    ExecutionDuration = timeJob.ElapsedMilliseconds,
                    RegExError = regExError

                };
                Console.WriteLine($"||Job {job.Url} BAD response");
                Console.WriteLine($"||Status: {jobresp.Status}");
                result.ResponsesList.Add(jobresp);
            }

            timeAll.Stop();


            //log
            Console.Write($"||Shedule with id {sch.BigJobId}: ");
            if (result.Status != "Bad Response" && result.Status != "RegEx Error")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                result.Status = "Everything OK";
                Console.Write($"{result.Status}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{result.Status}");
            }
            Console.WriteLine("\n");
            Console.ResetColor();
            //log


            result.ExecutionDuration = timeAll.ElapsedMilliseconds;
            result.EndTime = DateTime.Now.AddMilliseconds(0);
            await _signalRAgent.SendResponse(result);

            return result;
        }


        private bool CheckRegEx(string? succEx, string? errEx, string input)
        {

            bool succFound = false;
            bool errFound = false;

            if (succEx != "")
            {
                foreach (Match match in Regex.Matches(input, succEx))
                {
                    if (match.Success)
                    {
                        succFound = true;
                        break;
                    }
                }
            }
            else succFound = true;

            if (errEx != "")
            {
                foreach (Match match in Regex.Matches(input, errEx))
                {
                    if (match.Success)
                    {
                        errFound = true;
                        break;
                    }
                }
            }
            else errFound = false;

            return succFound || !errFound;
        }

    }
}
