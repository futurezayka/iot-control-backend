using IotControlService.Services;
using Quartz;

namespace IotControlService.Jobs;

public class ReceiveDataJob : IJob
{
    private readonly RabbitMqService _rabbitMqService;

    public ReceiveDataJob(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public async Task Execute(IJobExecutionContext context)
    {

        var deviceId = context.MergedJobDataMap.GetString("deviceId")!;
        var cancellationToken = context.CancellationToken;

        try
        {
            await _rabbitMqService.StartConsumingDataAsync(deviceId, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Job for device {deviceId} was cancelled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in job for device {deviceId}: {ex.Message}");
        }
    }
}