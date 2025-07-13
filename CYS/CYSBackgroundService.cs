using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CYS
{
    public class CYSBackgroundService : BackgroundService
    {
        private readonly ILogger<CYSBackgroundService> _logger;
        private readonly IWebHost _webHost;

        public CYSBackgroundService(ILogger<CYSBackgroundService> logger)
        {
            _logger = logger;
            _webHost = CreateWebHostBuilder().Build();
        }

        private IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:5001")
                .ConfigureServices(services =>
                {
                    // Gerekli servisleri burada ekleyin
                    services.AddControllers();
                    services.AddEndpointsApiExplorer();
                    services.AddDistributedMemoryCache();
                    services.AddSession(so =>
                    {
                        so.IdleTimeout = TimeSpan.FromSeconds(6000);
                    });
                })
                .Configure(app =>
                {
                    // Middleware ve app yapılandırması
                    app.UseRouting();
                    app.UseSession();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    
                    // Windows desteği varsa EventLog ekle
                    if (OperatingSystem.IsWindows())
                    {
                        logging.AddEventLog(settings =>
                        {
                            settings.SourceName = "CYS Livestock Service";
                        });
                    }
                });
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CYS Livestock Service başlatılıyor...");
            await _webHost.StartAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("CYS Livestock Service çalışıyor: {time}", DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CYS Livestock Service'de hata oluştu");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CYS Livestock Service durduruluyor...");
            await _webHost.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
} 