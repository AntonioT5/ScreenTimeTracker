using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Repository.Interface;
using Service.DTOs.SessionSyncDtos;
using Service.Interface;

namespace Service.Implementation
{
    public class AppSessionService : IAppSessionService
    {
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<AppSession> _sessionRepository;

        public AppSessionService(IRepository<Device> deviceRepository, IRepository<AppSession> sessionRepository)
        {
            _deviceRepository = deviceRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<int> IngestBatchAsync(string apiKey, SessionBatchRequest request)
        {
            var device = await _deviceRepository.Get(
                selector: d => d,
                predicate: d=>d.ApiKey==apiKey
            );

            if (device is null)
            {
                throw new UnauthorizedAccessException("Invalid device API key.");
            }

            if (request.Sessions.Count == 0)
            {
                return 0;
            }

            var incomingStartTimes = request.Sessions.Select(s => s.StartTime).ToList();

            var existingStartTimes = (await _sessionRepository.GetAllAsync(
                selector: s => s.StartTime,
                predicate: s => s.DeviceId == device.Id && incomingStartTimes.Contains(s.StartTime)
            )).ToHashSet();

            var newSessions = request.Sessions
                .Where(s => !existingStartTimes.Contains(s.StartTime))
                .Select(s => new AppSession
                {
                    DeviceId = device.Id,
                    ProcessName = s.ProcessName,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                })
                .ToList();

            if (newSessions.Count == 0)
            {
                return 0;
            }

            await _sessionRepository.InsertManyAsync(newSessions);
            return newSessions.Count;
        }
    }
}