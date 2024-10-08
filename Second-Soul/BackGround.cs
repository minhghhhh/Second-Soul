
using BusssinessObject;
using Data;

namespace Second_Soul
{
    public class BackGround : BackgroundService
    {
        private readonly ILogger<BackGround> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackGround(ILogger<BackGround> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider; 
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Service is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                        await unitOfWork.CouponRepository.DisableExpiredCoupons();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disable Expired Coupons: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }

        }
    }
}