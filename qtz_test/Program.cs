using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace qtz_test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting up");

            // construct a scheduler factory
            var factory = new StdSchedulerFactory();

            // get a scheduler
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            var jobRunner = new JobRunner();

            //await jobRunner.ImmediatelyStartJob(scheduler);

            //await jobRunner.TryScheduleAJobTwice(scheduler);

            await jobRunner.ScheduleAfterDelay(scheduler);

            Console.ReadLine();

            await scheduler.Shutdown(waitForJobsToComplete: true);
        }
    }
}
