using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;

namespace qtz_test
{
    public class JobRunner
    {
        public async Task ImmediatelyStartJob(IScheduler scheduler)
        {
            // define the job and tie it to our job class
            var jobDetail = JobBuilder.Create<SimpleJob>()
                .WithIdentity("immediate started job", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            var trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);
        }

        public async Task TryScheduleAJobTwice(IScheduler scheduler)
        {
            var jobDetail1 = JobBuilder.Create<DoubleUpJob>()
                .WithIdentity("double up job", "group1")
                .Build();

            var jobDetail2 = JobBuilder.Create<DoubleUpJob>()
                .WithIdentity("double up job", "group1")
                .Build();

            var trigger1 = TriggerBuilder.Create()
                .WithIdentity("myTrigger1", "group1")
                .StartAt(DateTimeOffset.Now.Add(TimeSpan.FromSeconds(5)))
                .Build();

            await scheduler.ScheduleJob(jobDetail1, trigger1);

            await Task.Delay(TimeSpan.FromSeconds(1));

            var trigger2 = TriggerBuilder.Create()
                .WithIdentity("myTrigger2", "group1")
                .StartAt(DateTimeOffset.Now.Add(TimeSpan.FromSeconds(5)))
                .Build();

            await scheduler.ScheduleJob(jobDetail2, trigger2);
        }

        public async Task ScheduleAfterDelay(IScheduler scheduler)
        {
            var listener = new
               RepeatAfterCompletionJobListener(TimeSpan.FromSeconds(5));

            scheduler.ListenerManager.AddJobListener
                     (listener, KeyMatcher<JobKey>.KeyEquals(new JobKey("re", "group1")));

            var job = JobBuilder.Create<SimpleJob>()
                            .WithIdentity("re", "group1")
                            .Build();

            // Schedule the job to start in 5 seconds to give the service time to initialise
            var trigger = TriggerBuilder.Create()
                            .WithIdentity("trigger_re", "group1")
                            .StartNow()
                            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
