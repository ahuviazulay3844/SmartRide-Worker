using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;

namespace CityCar.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 CityCar Worker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // יצירת Scope כדי שנוכל להשתמש בשירותים שמצפים ל-DB (כמו OrderService)
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var trackingService = scope.ServiceProvider.GetRequiredService<OrderTrackingService>();
                        await trackingService.RunOnce();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error occurred in Worker execution loop");
                }

                // המתנה של 60 שניות לפני הבדיקה הבאה
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}