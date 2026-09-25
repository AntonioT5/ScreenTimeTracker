using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.DTOs.RequestResponse;
using Service.Interface;

namespace Service.Implementation
{
    public class SummaryService : ISummaryService
    {
        private readonly IRepository<AppSession> _sessionRepository;
        private readonly IRepository<DailySummary> _summaryRepository;
        private readonly ILogger<SummaryService> _logger;

        public SummaryService(IRepository<AppSession> sessionRepository, IRepository<DailySummary> summaryRepository, ILogger<SummaryService> logger)
        {
            _sessionRepository = sessionRepository;
            _summaryRepository = summaryRepository;
            _logger = logger;
        }

        public async Task AggregateDailyAsync(DateTime date)
        {
            var dateOnly = DateOnly.FromDateTime(date.Date);

            var alreadyExists = await _summaryRepository.Get(
                selector: s => s.Id,
                predicate: s => s.Date == dateOnly);

            if (alreadyExists != Guid.Empty)
            {
                _logger.LogInformation("Skipping {Date}, already aggregated", dateOnly);
                return;
            }

            var dayStart = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var dayEnd = dayStart.AddDays(1);

            var raw = await _sessionRepository.GetAllAsync(
                selector: s => new
                {
                    UserId = s.Device.UserId,
                    s.ProcessName,
                    s.StartTime,
                    s.EndTime
                },
                predicate: s => s.StartTime >= dayStart && s.StartTime < dayEnd);

            var grouped = raw
                .GroupBy(s => new { s.UserId, s.ProcessName })
                .Select(g => new DailySummary
                {
                    UserId = g.Key.UserId,
                    ProcessName = g.Key.ProcessName,
                    Date = dateOnly,
                    DurationSeconds = (int)g.Sum(s => (s.EndTime - s.StartTime).TotalSeconds)
                })
                .ToList();

            if (grouped.Count > 0)
            {
                await _summaryRepository.InsertManyAsync(grouped);
            }
        }

        public async Task<DateTime?> GetFirstSessionDateAsync()
        {
            return await _sessionRepository.Get(
                selector: s => (DateTime?)s.StartTime,
                orderBy: q => q.OrderBy(s => s.StartTime));
        }

        public async Task<DateOnly?> GetLastAggregatedDateAsync()
        {
            return await _summaryRepository.Get(
                selector: s => (DateOnly?)s.Date,
                orderBy: q => q.OrderByDescending(s => s.Date));
        }

        public async Task<SummaryResponse> GetSummaryAsync(Guid userId, int? days)
        {
            DateTime? passDays = days.HasValue ? DateTime.UtcNow.AddDays(-days.Value) : null;
        
            var raw = await _sessionRepository.GetAllAsync(
                selector: s => new
                {
                    s.DeviceId,
                    DeviceName = s.Device.DeviceName,
                    s.ProcessName,
                    s.StartTime,
                    s.EndTime
                },
                predicate: passDays.HasValue
                    ? s => s.Device.UserId == userId && s.StartTime >= passDays.Value
                    : s => s.Device.UserId == userId
            );

            var withDuration = raw.Select(s => new
                {
                    s.DeviceId,
                    s.DeviceName,
                    s.ProcessName,
                    DurationSeconds = (int)(s.EndTime - s.StartTime).TotalSeconds
                }).ToList();

            var overall = withDuration.GroupBy(s=>s.ProcessName)
                .Select(p => new AppUsageDto
                {
                    ProcessName = p.Key,
                    DurationSeconds = p.Sum(x => x.DurationSeconds)
                })
                .OrderByDescending(s=>s.DurationSeconds)
                .ToList();

            var totalTimeSpend = withDuration.Sum(s=> (long)s.DurationSeconds);

            var byDevice = withDuration.GroupBy(s=> new { s.DeviceId, s.DeviceName })
                .Select(g => new DeviceSummaryDto
                {
                    DeviceId = g.Key.DeviceId,
                    DeviceName = g.Key.DeviceName,
                    Apps = g.GroupBy(x => x.ProcessName)
                        .Select(ag => new AppUsageDto
                        {
                            ProcessName = ag.Key,
                            DurationSeconds = ag.Sum(x => x.DurationSeconds)
                        })
                        .OrderByDescending(a => a.DurationSeconds)
                        .ToList()
                })
                .ToList();
            return new SummaryResponse { Overall = overall, ByDevice = byDevice, TotalTimeSpend=totalTimeSpend };
        }

        
    }
}