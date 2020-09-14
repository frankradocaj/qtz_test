using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace qtz_test
{
    public class Startup
    {
        public Startup()
        {
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddQuartz(configurator =>
            {
                // handy when part of cluster or you want to otherwise identify multiple schedulers
                configurator.SchedulerId = "my_scheduler";

                // we take this from appsettings.json, just show it's possible
                // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

                // we could leave DI configuration intact and then jobs need to have public no-arg constructor
                // the MS DI is expected to produce transient job instances 
                configurator.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    // if we don't have the job in DI, allow fallback to configure via default constructor
                    options.AllowDefaultConstructor = true;
                });

                // or 
                // q.UseMicrosoftDependencyInjectionScopedJobFactory();

                // these are the defaults
                configurator.UseSimpleTypeLoader();
                configurator.UseInMemoryStore();
                configurator.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                //// quickest way to create a job with single trigger is to use ScheduleJob (requires version 3.2)
                //configure.ScheduleJob<MyJob>(trigger => trigger
                //    .WithIdentity("Combined Configuration Trigger")
                //    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                //    .WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
                //    .WithDescription("my awesome trigger configured for a job with single call")
                //);

                //// you can also configure individual jobs and triggers with code
                //// this allows you to associated multiple triggers with same job
                //// (if you want to have different job data map per trigger for example)
                //configure.AddJob<ExampleJob>(j => j
                //    .StoreDurably() // we need to store durably if no trigger is associated
                //    .WithDescription("my awesome job")
                //);

                // here's a known job for triggers
                var jobKey = new JobKey("awesome job", "awesome group");
                configurator.AddJob<MyJob>(jobKey, j => j
                    .WithDescription("my awesome job")
                );

                configurator.AddTrigger(t => t
                    .WithIdentity("Simple Trigger")
                    .ForJob(jobKey)
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever())
                    .WithDescription("my awesome simple trigger")
                );

                configurator.AddTrigger(t => t
                    .WithIdentity("Cron Trigger")
                    .ForJob(jobKey)
                    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                    .WithCronSchedule("0/3 * * * * ?")
                    .WithDescription("my awesome cron trigger")
                );

                //// you can add calendars too (requires version 3.2)
                //const string calendarName = "myHolidayCalendar";
                //configure.AddCalendar<HolidayCalendar>(
                //    name: calendarName,
                //    replace: true,
                //    updateTriggers: true,
                //    x => x.AddExcludedDate(new DateTime(2020, 5, 15))
                //);

                configurator.AddTrigger(t => t
                    .WithIdentity("Daily Trigger")
                    .ForJob(jobKey)
                    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(5)))
                    .WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
                    .WithDescription("my awesome daily time interval trigger")
                    .ModifiedByCalendar(calendarName)
                );

                //// also add XML configuration and poll it for changes
                //configure.UseXmlSchedulingConfiguration(x =>
                //{
                //    x.Files = new[] { "~/quartz_jobs.config" };
                //    x.ScanInterval = TimeSpan.FromSeconds(2);
                //    x.FailOnFileNotFound = true;
                //    x.FailOnSchedulingError = true;
                //});

                //// convert time zones using converter that can handle Windows/Linux differences
                //configure.UseTimeZoneConverter();

                //// add some listeners
                //configure.AddSchedulerListener<SampleSchedulerListener>();
                //configure.AddJobListener<SampleJobListener>(GroupMatcher<JobKey>.GroupEquals(jobKey.Group));
                //configure.AddTriggerListener<SampleTriggerListener>();

                // example of persistent job store using JSON serializer as an example
                /*
                q.UsePersistentStore(s =>
                {
                    s.UseProperties = true;
                    s.RetryInterval = TimeSpan.FromSeconds(15);
                    s.UseSqlServer(sqlServer =>
                    {
                        sqlServer.ConnectionString = "some connection string";
                        // this is the default
                        sqlServer.TablePrefix = "QRTZ_";
                    });
                    s.UseJsonSerializer();
                    s.UseClustering(c =>
                    {
                        c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                        c.CheckinInterval = TimeSpan.FromSeconds(10);
                    });
                });
                */
            });
        }
    }
}
