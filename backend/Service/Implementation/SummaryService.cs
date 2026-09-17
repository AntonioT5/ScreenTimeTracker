using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Repository.Interface;
using Service.DTOs.RequestResponse;
using Service.Interface;

namespace Service.Implementation
{
    public class SummaryService : ISummaryService
    {
        private readonly IRepository<AppSession> _sessionRepository;

        public SummaryService(IRepository<AppSession> sessionRepository)
        {
            _sessionRepository = sessionRepository;
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
            return new SummaryResponse { Overall = overall, ByDevice = byDevice };
        }
    }
}