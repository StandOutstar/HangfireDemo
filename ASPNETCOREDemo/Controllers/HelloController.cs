using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCOREDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : Controller
    {
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        // GET
        [HttpGet]
        public IActionResult Get()
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Get Hello!"));
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult AddRecurringJob(string id)
        {
            RecurringJob.AddOrUpdate(id, () => Work(id), Cron.Minutely, TimeZoneInfo.Local,"recurringQueue");
            return Ok($"Recurring Job {id} start.");
        }

        [HttpGet("long/")]
        public IActionResult RunLongRunningJob()
        {
            var id = BackgroundJob.Enqueue(() => LongRunningMethod(_cancellationTokenSource.Token));
            return Ok($"LongRunningJob {id} Starts.");
        }

        [HttpGet("long/cancel")]
        public IActionResult CancelLongRunningJob()
        {
            _cancellationTokenSource.Cancel();
            return Ok("LongRunningJob Canceled.");
        }

        [HttpGet("background/job/cancel/{id}")]
        public IActionResult CancelBackgroundJob(string id)
        {
            BackgroundJob.Delete(id);
            return Ok($"RunningJob {id} Deleted.");
        }

        [HttpGet("recurring/job/cancel/{id}")]
        public IActionResult CancelRecurringJob(string id)
        {
            RecurringJob.RemoveIfExists(id);
            return Ok($"RunningJob {id} Deleted.");
        }
        
        public void Work(string id)
        {
            var now = DateTimeOffset.Now;
            Console.WriteLine($"Hangfire job {id} run at {now:hh:mm:ss.fff}"); 
        }

        public async Task LongRunningMethod(CancellationToken token)
        {
            Console.WriteLine($"Start run LongRunningJob at {DateTimeOffset.Now:hh:mm:ss.fff}"); 
            await Task.Delay(TimeSpan.FromHours(1), token);
            Console.WriteLine($"Cancelled  LongRunningJob at {DateTimeOffset.Now:hh:mm:ss.fff}");
        }
    }
}