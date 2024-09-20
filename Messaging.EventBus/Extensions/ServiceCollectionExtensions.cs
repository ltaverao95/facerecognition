namespace Messaging.Bus.Extensions
{
    using Messaging.EventBus.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class ServiceCollectionExtensions
    {
        public static void AddMessagingEventBusStartup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHostedService, BusService>();
        }
    }
}