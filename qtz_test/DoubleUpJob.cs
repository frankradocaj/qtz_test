using System;
using System.Threading.Tasks;
using Quartz;

namespace qtz_test
{
    [DisallowConcurrentExecution]
    public class DoubleUpJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync($"'{context.JobDetail.Key.Name}' is executing");

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}
