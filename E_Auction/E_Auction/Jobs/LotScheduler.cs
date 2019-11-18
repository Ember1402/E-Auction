using E_Auction.Context;
using E_Auction.Enums;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Auction.Jobs
{
    public class LotScheduler
    {
        const int DEFAULT_LOT_TRIGGER_INTERVAL = 60;
        public static async void Start()
        {
            string parsedInterval;

            using (var db=new AuctionContext())
            {
                var configId = (int)ConfigEnum.LotTriggerInterval;

                var lotIntervalConfig = await db.Configs.FindAsync(configId);

                parsedInterval = lotIntervalConfig?.Value ?? "";
            }

            //

            bool parseResult = int.TryParse(parsedInterval, out int lotTriggerInterval);

            if (parseResult==false) lotTriggerInterval = DEFAULT_LOT_TRIGGER_INTERVAL;
            

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<LotCloser>().Build();
            //IJobDetail job2 = JobBuilder.Create<EmailService>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(lotTriggerInterval)
                    .RepeatForever())
                .Build();


            await scheduler.ScheduleJob(job, trigger);
            //await scheduler.ScheduleJob(job2, trigger);
        }
    }
}