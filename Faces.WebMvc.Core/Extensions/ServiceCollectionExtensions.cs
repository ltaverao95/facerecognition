namespace Faces.WebMvc.Core.Extensions
{
    using Faces.WebMvc.Core.Models;
    using Faces.WebMvc.Core.Repositories;
    using Faces.WebMvc.Core.Repositories.Impl;
    using Faces.WebMvc.Core.RestClient;
    using Faces.WebMvc.Core.RestClient.Impl;
    using Faces.WebMvc.Core.Services;
    using Faces.WebMvc.Core.Services.Impl;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddCoreStartup(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FacesWebConfiguration>(configuration);

            services.AddMassTransit(busRegistrationConfigurator =>
            {
                busRegistrationConfigurator.UsingRabbitMq((context, rabbitBusFactoryConfigurator) =>
                {
                    rabbitBusFactoryConfigurator.Host("rabbitmq", configuration["RabbitConfig:VHost"]!, rabbitHostConfigurator =>
                    {
                        rabbitHostConfigurator.Username(configuration["RabbitConfig:UserName"]!);
                        rabbitHostConfigurator.Password(configuration["RabbitConfig:Password"]!);
                    });
                });

                services.AddSingleton(busRegistrationConfigurator);
            });

            services.AddHttpClient<IOrderManagementApi, OrderManagementApi>();

            services.AddScoped<IRegisterOrderService, RegisterOrderService>();
            services.AddScoped<IOrdersRepository, OrdersRepository>();
        }
    }
}