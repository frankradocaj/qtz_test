using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace qtz_test
{
    public class RepeatAfterCompletionJobListener : IJobListener
    {
        private readonly TimeSpan interval;

        public RepeatAfterCompletionJobListener(TimeSpan interval)
        {
            this.interval = interval;
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
            string triggerKey = context.JobDetail.Key.Name + ".trigger";

            var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey, "group1")
                    .StartAt(DateTimeOffset.Now.AddSeconds(5))
                    .Build();

            await context.Scheduler.RescheduleJob(new TriggerKey(triggerKey), trigger);
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
