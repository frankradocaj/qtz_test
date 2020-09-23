using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace qtz_test
{
    public class RepeatAfterCompletionJobListener : IJobListener
    {
        private readonly int maxRunCount = 0;

        public RepeatAfterCompletionJobListener(int maxRunCount)
        {
            this.maxRunCount = maxRunCount;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            var runCount = context.Trigger.JobDataMap.GetIntValue("count");
            Console.Out.Write(runCount);

            if (runCount == maxRunCount)
            {
                return;
            }

            var trigger = TriggerBuilder.Create()
                    .WithIdentity(context.Trigger.Key.Name, "loadgroup")
                    .UsingJobData("count", ++runCount)
                    .StartAt(DateTimeOffset.Now.AddSeconds(5))  
                    .Build();

            await context.Scheduler.RescheduleJob(context.Trigger.Key, trigger);
        }

        public string Name
        {
            get
            {
                return "RepeatAfterCompletionJobListener";
            }
        }
    }
}
