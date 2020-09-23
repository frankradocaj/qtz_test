using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;

namespace qtz_test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"---- {DateTime.Now} App started");

            // construct a scheduler factory
            var settings = new NameValueCollection();
            settings.Add("quartz.threadPool.maxConcurrency", "10");
            var factory = new StdSchedulerFactory(settings);

            // get a scheduler
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            var jobRunner = new JobRunner();

            //await jobRunner.ImmediatelyStartJob(scheduler);

            //await jobRunner.TryScheduleAJobTwice(scheduler);

            //await jobRunner.ScheduleAfterDelay(scheduler);

            await jobRunner.ScheduleManyJobs(scheduler);

            Console.ReadLine();

            await scheduler.Shutdown(waitForJobsToComplete: true);
        }
    }
}
