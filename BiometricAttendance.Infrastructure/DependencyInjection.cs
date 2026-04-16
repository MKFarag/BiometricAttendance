using BiometricAttendance.Application.Settings;
using BiometricAttendance.Infrastructure.Messaging;
using BiometricAttendance.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BiometricAttendance.Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureDependencies(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("DefaultConnection is not found in appsettings.json");

            services.AddHttpContextAccessor();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddHybridCache();

            services.AddStackExchangeRedisCache(options => options.Configuration = configuration.GetConnectionString("Redis"));

            services.AddCQRSConfig(typeof(INotificationService).Assembly);

            services.AddOptions<EnrollmentCommands>()
                .BindConfiguration(EnrollmentCommands.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            //services.AddSingleton<ISerialPortService>(provider =>
            //    new SerialPortService("COM3", 9600, provider.GetRequiredService<ILogger<SerialPortService>>()));

            return services;
        }

        private IServiceCollection AddCQRSConfig(params Assembly[] assemblies)
        {
            services.AddScoped<ISender, Sender>();

            if (assemblies.Length == 0)
                assemblies = [Assembly.GetCallingAssembly()];

            foreach (var assembly in assemblies)
            {
                var handlers = assembly.GetTypes()
                    .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericType: false })
                    .SelectMany(t => t.GetInterfaces()
                        .Where(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
                        .Select(i => (Interface: i, Implementation: t)));

                foreach (var (iface, impl) in handlers)
                    services.AddScoped(iface, impl);
            }

            return services;
        }
    }
}
