using IotControlService.Jobs;
using IotControlService.Services;
using Quartz;

namespace IotControlService.Helpers;

public class JobHelper
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly RabbitMqService _rabbitMqService;

    public JobHelper(ISchedulerFactory schedulerFactory, RabbitMqService rabbitMqService)
    {
        _schedulerFactory = schedulerFactory;
        _rabbitMqService = rabbitMqService;
    }

    public async Task StartCollectingData(string deviceId, string deviceType)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"ReceiveDataJob-{deviceId}");
        await _rabbitMqService.SendCommandAsync(deviceId: deviceId, command: "start_stream", deviceType: deviceType);
        var jobDetail = JobBuilder
            .Create<ReceiveDataJob>()
            .WithIdentity(jobKey)
            .UsingJobData("deviceId", deviceId)
            .Build();


        var trigger = TriggerBuilder
            .Create()
            .ForJob(jobDetail)
            .WithIdentity($"BackgroundJob-trigger-{deviceId}")
            .StartNow()
            .Build();


        await scheduler.ScheduleJob(jobDetail, trigger);
    }

    public async Task StopCollectingData(string deviceId, string deviceType)
    {
        await _rabbitMqService.SendCommandAsync(deviceId: deviceId, command: "stop_stream", deviceType: deviceType);
        var jobKey = new JobKey($"ReceiveDataJob-{deviceId}");
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.Interrupt(jobKey);
        await scheduler.DeleteJob(jobKey);
    }
}