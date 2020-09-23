using System;
using System.Threading;
using Hangfire;
using Hangfire.MemoryStorage;

namespace HangfireDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hangfire Start!");
            
            // 使用内存数据库
            GlobalConfiguration.Configuration.UseMemoryStorage();
            // or 使用其他数据库并填写连接字符串
            // GlobalConfiguration.Configuration.UseSQLiteStorage("<>connection string>");
            
            // 创建 Hangfire Server，自动调用 Dispose
            using var server = new BackgroundJobServer();
            
            // 添加循环执行的定时任务
            // 如果 Work() 替换为 Console.WriteLine($"Hangfire  AddOrUpdate任务 run at {now:hh:mm:ss.fff}")，
            // 发现每次输出的时间都相同
            RecurringJob.AddOrUpdate("first-id",() => Work(), Cron.Minutely, TimeZoneInfo.Local);
            
            // 添加一次性任务
            BackgroundJob.Enqueue(() => Console.WriteLine($"background start at {DateTimeOffset.Now}"));
            
            Console.ReadLine();
        }

        public static void Work()
        {
            var now = DateTimeOffset.Now;
            Console.WriteLine($"Hangfire  AddOrUpdate任务 run at {now:hh:mm:ss.fff}"); 
        }
    }
}