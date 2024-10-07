
using BusssinessObject;

namespace Second_Soul
{
    public class BackGroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public BackGroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Create a scope to resolve services
                using (var scope = _serviceProvider.CreateScope())
                {
                    var couponService = scope.ServiceProvider.GetRequiredService<CouponBusiness>();
                    await couponService.DisableExpiredCoupons();
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
