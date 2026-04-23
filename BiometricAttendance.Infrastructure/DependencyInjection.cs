using BiometricAttendance.Infrastructure.Fingerprint;
using BiometricAttendance.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddFingerprintConfig(configuration);

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

        private IServiceCollection AddFingerprintConfig(IConfiguration configuration)
        {
            services.AddOptions<EnrollmentCommands>()
                .BindConfiguration(EnrollmentCommands.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<ISerialPortService>(provider =>
            {
                var portName = configuration["SerialPort:PortName"]!;
                var baudRate = configuration.GetValue<int>("SerialPort:BaudRate");
                var logger = provider.GetRequiredService<ILogger<SerialPortService>>();

                return new SerialPortService(portName, baudRate, logger);
            });

            services.AddSingleton<FingerprintStatus>();

            services.AddScoped<IFingerprintService, FingerprintService>();

            return services;
        }
    }
}
