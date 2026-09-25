using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Service.BackgroundJob
{
    public class DailyAggregationJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailyAggregationJob> _logger;

        public DailyAggregationJob(IServiceScopeFactory scopeFactory, ILogger<DailyAggregationJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var summaryService = scope.ServiceProvider.GetRequiredService<ISummaryService>();
                await CatchUpMissedDaysAsync(summaryService, stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1).AddHours(1);
                var delay = nextRun - now;

                await Task.Delay(delay, stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var summaryService = scope.ServiceProvider.GetRequiredService<ISummaryService>();

                try
                {
                    var yesterday = DateTime.UtcNow.Date.AddDays(-1);
                    await summaryService.AggregateDailyAsync(yesterday);
                    _logger.LogInformation("Aggregated daily summary for {Date}", yesterday);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Daily aggregation failed");
                }
            }
        }

        private async Task CatchUpMissedDaysAsync(ISummaryService summaryService, CancellationToken stoppingToken)
        {
            var lastAggregatedDate = await summaryService.GetLastAggregatedDateAsync();
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);

            DateTime startDate;
            if (lastAggregatedDate is null)
            {
                var firstSessionDate = await summaryService.GetFirstSessionDateAsync();
                if (firstSessionDate is null)
                    return;

                startDate = firstSessionDate.Value.Date;
            }
            else
            {
                startDate = lastAggregatedDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(1);
            }

            for (var d = startDate; d <= yesterday; d = d.AddDays(1))
            {
                if (stoppingToken.IsCancellationRequested) break;

                await summaryService.AggregateDailyAsync(d);
                _logger.LogInformation("Caught up day: {Date}", d);
            }
        }
    }
}