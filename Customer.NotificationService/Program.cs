namespace Notification.Customer.NotificationService
{
    using MassTransit;
    using Messaging.Bus.Extensions;
    using Messaging.Core.Constants;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Notification.Customer.EmailService.Models.Configurations;
    using Notification.Customer.EmailService.Senders;
    using Notification.Customer.EmailService.Senders.Impl;
    using Notification.Customer.NotificationService.Core.Consumers;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile($"appsettings.json", optional: false);
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    var emailConfig = hostContext.Configuration
                    .GetSection("EmailConfiguration")
                    .Get<EmailConfig>();

                    services.AddSingleton(emailConfig);
                    services.AddScoped<IEmailSender, EmailSender>();

                    services.AddMassTransit(busRegistrationConfigurator =>
                    {
                        busRegistrationConfigurator.AddConsumer<OrderProcessedEventConsumer>();

                        busRegistrationConfigurator.UsingRabbitMq((context, rabbitBusFactoryConfigurator) =>
                        {
                            rabbitBusFactoryConfigurator.Host("rabbitmq", configuration["RabbitConfig:VHost"]!, rabbitHostConfigurator =>
                            {
                                rabbitHostConfigurator.Username(configuration["RabbitConfig:UserName"]!);
                                rabbitHostConfigurator.Password(configuration["RabbitConfig:Password"]!);
                            });

                            rabbitBusFactoryConfigurator.ReceiveEndpoint(RabbitMqMassTransitConstants.NotificationServiceQueue, rabbitReceiveEndPointConfigurator =>
                            {
                                rabbitReceiveEndPointConfigurator.PrefetchCount = 16;
                                rabbitReceiveEndPointConfigurator.UseMessageRetry(y => y.Interval(2, TimeSpan.FromSeconds(10)));
                                rabbitReceiveEndPointConfigurator.ConfigureConsumer<OrderProcessedEventConsumer>(context);
                            });

                            rabbitBusFactoryConfigurator.ConfigureEndpoints(context);
                        });

                        services.AddSingleton(busRegistrationConfigurator);
                    });

                    services.AddMessagingEventBusStartup(hostContext.Configuration);
                });

            return hostBuilder;
        }
    }
}