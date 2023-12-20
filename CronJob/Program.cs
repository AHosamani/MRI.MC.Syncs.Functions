using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        await ScheduleJob();
    }

    private static async Task ScheduleJob()
    {
        // Set up Quartz scheduler
        var schedulerFactory = new StdSchedulerFactory();
        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.Start();

        // Initial CRON expression (you can fetch this from the database)
        string cronExpression = GetCronExpressionFromDatabase();

        // Define the job and tie it to your job class
        var job = JobBuilder.Create<YourJobClass>()
            .WithIdentity("myJob", "group1")
            .Build();

        // Trigger the job with the initial CRON expression
        var trigger = TriggerBuilder.Create()
            .WithIdentity("myTrigger", "group1")
            .WithCronSchedule(cronExpression)
            .Build();

        // Schedule the job using the trigger
        await scheduler.ScheduleJob(job, trigger);

        // Continuously check for changes in CRON expression and reschedule
        while (true)
        {
            await Task.Delay(TimeSpan.FromMinutes(1)); // Adjust the delay based on your requirements

            string newCronExpression = GetCronExpressionFromDatabase();

            if (cronExpression != newCronExpression)
            {
                // Reschedule the job with the updated CRON expression
                await scheduler.RescheduleJob(trigger.Key, TriggerBuilder.Create()
                    .WithIdentity("myTrigger", "group1")
                    .WithCronSchedule(newCronExpression)
                    .Build());

                cronExpression = newCronExpression;
            }
        }
    }

    private static string GetCronExpressionFromDatabase()
    {
        List<string> ids = new List<string>();
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        IConfiguration configuration = configurationBuilder.Build();
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT [CronExpression] FROM [Cron].[dbo].[CronEx]";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                //Execute the query
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Access data from the reader
                        //ids.Append(reader.GetString(0));
                        string propertyId = reader.GetString(0);
                        ids.Add(propertyId);
                    }
                }
            }
        }
        return ids[0];
    }
}

public class YourJobClass : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Job executed at: {DateTime.Now}");
        return Task.CompletedTask;
    }
}
