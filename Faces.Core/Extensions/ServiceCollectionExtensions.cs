namespace Faces.Core.Extensions
{
    using Faces.Core.Services;
    using Faces.Core.Services.Impl;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddCoreStartup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGetFacesFromImageService, GetFacesFromImageService>();
        }
    }
}