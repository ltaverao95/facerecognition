namespace Orders.Core.Extensions
{
    using MassTransit;
    using Messaging.Core.Constants;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Orders.Core.Messages.Consumers;
    using Orders.Core.Models.Configuration;
    using Orders.Core.Persistence;
    using Orders.Core.Persistence.Repository;
    using Orders.Core.Persistence.Repository.Impl;

    public static class ServiceCollectionExtensions
    {
        public static void AddCoreStartup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddDbContext<OrdersContext>(dbContextOptionsBuilder =>
            {
                dbContextOptionsBuilder.UseSqlServer(configuration["OrdersContextConnection"], opt =>
                {
                    opt.MigrationsAssembly("Orders.API");
                });
            });

            services.Configure<OrderSettingsConfiguration>(configuration);

            services.AddMassTransit(busRegistrationConfigurator =>
            {
                busRegistrationConfigurator.AddConsumer<RegisterOrderCommandConsumer>();
                busRegistrationConfigurator.AddConsumer<OrderDispatchedEventConsumer>();

                busRegistrationConfigurator.UsingRabbitMq((context, rabbitBusFactoryConfigurator) =>
                {
                    rabbitBusFactoryConfigurator.Host("rabbitmq", configuration["RabbitConfig:VHost"]!, rabbitHostConfigurator =>
                    {
                        rabbitHostConfigurator.Username(configuration["RabbitConfig:UserName"]!);
                        rabbitHostConfigurator.Password(configuration["RabbitConfig:Password"]!);
                    });

                    rabbitBusFactoryConfigurator.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, rabbitReceiveEndPointConfigurator =>
                    {
                        rabbitReceiveEndPointConfigurator.PrefetchCount = 16;
                        rabbitReceiveEndPointConfigurator.UseMessageRetry(y => y.Interval(2, TimeSpan.FromSeconds(10)));
                        rabbitReceiveEndPointConfigurator.ConfigureConsumer<RegisterOrderCommandConsumer>(context);
                    });

                    rabbitBusFactoryConfigurator.ReceiveEndpoint(RabbitMqMassTransitConstants.OrderDispatchedServiceQueue, rabbitReceiveEndPointConfigurator =>
                    {
                        rabbitReceiveEndPointConfigurator.PrefetchCount = 16;
                        rabbitReceiveEndPointConfigurator.UseMessageRetry(y => y.Interval(2, TimeSpan.FromSeconds(10)));
                        rabbitReceiveEndPointConfigurator.ConfigureConsumer<OrderDispatchedEventConsumer>(context);
                    });

                    rabbitBusFactoryConfigurator.ConfigureEndpoints(context);
                });

                services.AddSingleton(busRegistrationConfigurator);
            });

            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}