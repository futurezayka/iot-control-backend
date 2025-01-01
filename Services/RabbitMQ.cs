using System.Text;
using System.Text.Json;
using IotControlService.Models;
using IotControlService.Repositories.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace IotControlService.Services
{
    public class RabbitMqService : IAsyncDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private IUnitOfWork _unitOfWork;

        public static async Task<RabbitMqService> CreateAsync(
            IUnitOfWork unitOfWork,
            string hostName = "rabbitmq",
            string userName = "rmuser",
            string password = "rmpassword"
        )
        {
            if (hostName == null) throw new ArgumentNullException(nameof(hostName));
            var instance = new RabbitMqService();
            instance._unitOfWork = unitOfWork;
            await instance.CreateConnectionFactoryAsync(hostName, userName, password);
            return instance;
        }

        private async Task CreateConnectionFactoryAsync(string hostName, string userName, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task SendCommandAsync(string deviceId, string command, string deviceType)
        {
            if (_channel == null) throw new InvalidOperationException("RabbitMqService is not initialized.");

            await _channel.QueueDeclareAsync(queue: "command_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var message = JsonSerializer.Serialize(new { type = command, device_id = deviceId, device_type = deviceType });
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(exchange: "",
                routingKey: "command_queue",
                body: body);

            Console.WriteLine($"[x] Sent command to device {deviceId}: {command}");
        }

        public async Task StartConsumingDataAsync(string deviceId, CancellationToken cancellationToken)
        {
            if (_channel == null) throw new InvalidOperationException("RabbitMqService is not initialized.");

            var queueName = $"data:{deviceId}:queue";

            await _channel.QueueDeclareAsync(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            
            await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer,
                cancellationToken: cancellationToken);
            
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"[x] Received data for device {deviceId}: {message}");
                    var deviceData = JsonSerializer.Deserialize<DeviceData>(message, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    })!;
                    deviceData.DeviceId = Guid.Parse(deviceId);
                    deviceData.Date = deviceData.Date.ToUniversalTime();
                    Console.WriteLine(
                        $"[x] Parsed data for device {deviceData.DeviceId}: {deviceData.Telemetry}, {deviceData.Date}");
                    await _unitOfWork.DeviceDataRepository.AddAsync(deviceData);
                    await _unitOfWork.SaveAsync();
                    await Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Error processing message: {ex.Message}");
                }
            };


            Console.WriteLine($"[*] Waiting for messages in {queueName}.");

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"[*] Consumer for {queueName} has been cancelled.");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}