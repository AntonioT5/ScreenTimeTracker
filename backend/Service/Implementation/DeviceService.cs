using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Repository.Interface;
using Service.DTOs.RequestResponse;
using Service.Interface;

namespace Service.Implementation
{
    public class DeviceService : IDeviceService
    {
        private readonly IRepository<Device> _repository;
        private readonly IRepository<User> _userRepository;
        private readonly IMemoryCache _cache;

        public DeviceService(IRepository<Device> repository, IRepository<User> userRepository, IMemoryCache cache)
        {
            _repository = repository;
            _userRepository = userRepository;
            _cache = cache;
}

        public async Task<RegisterDeviceResponse> RegisterDeviceAsync(RegisterDeviceRequest request)
        {
            var user = await _userRepository.Get(
                selector: u => u,
                predicate: u => u.Username == request.Username);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            return await GetOrCreateDeviceAsync(user.Id, request.DeviceName, request.OperatingSystem);
        }

        private async Task<RegisterDeviceResponse> GetOrCreateDeviceAsync(Guid userId, string deviceName, string operatingSystem)
        {
            deviceName = deviceName?.Trim() ?? "";
            if (deviceName.Length == 0)
            {
                throw new ArgumentException("Device name is required.");
            }

            var existingDevice = await _repository.Get(
                selector: d => d,
                predicate: d => d.UserId == userId && d.DeviceName == deviceName);

            if (existingDevice is not null)
            {
                return new RegisterDeviceResponse
                {
                    ApiKey = existingDevice.ApiKey,
                    DeviceName = existingDevice.DeviceName
                };
            }

            var device = new Device
            {
                UserId = userId,
                DeviceName = deviceName,
                OperatingSystem = operatingSystem,
                ApiKey = GenerateApiKey(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.InsertAsync(device);

            return new RegisterDeviceResponse
            {
                DeviceName = device.DeviceName,
                ApiKey = device.ApiKey
            };
        }

        private static string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }

        private class PendingDevice
        {
            public string DeviceName { get; init; } = "";
            public string OperatingSystem { get; init; } = "";
            public string? ApiKey { get; set; }
        }

        public void CreatePending(PendingDeviceRequest request)
        {
            var name = request.DeviceName?.Trim() ?? "";
            var os = request.OperatingSystem?.Trim() ?? "";

            if (request.Code is null || request.Code.Length < 16 || request.Code.Length > 64)
            {
                throw new ArgumentException("Invalid code.");
            }
            if (name.Length == 0 || name.Length > 100 || os.Length > 50)
            {
                throw new ArgumentException("Invalid device data.");
            }

            _cache.Set($"pending-device:{request.Code}", new PendingDevice { DeviceName = name, OperatingSystem = os }, TimeSpan.FromMinutes(10));
        }

        public PendingDeviceInfo? GetPendingInfo(string code)
        {
            if (!_cache.TryGetValue($"pending-device:{code}", out PendingDevice? pending) || pending is null)
            {
                return null;
            }

            return new PendingDeviceInfo
            {
                DeviceName = pending.DeviceName,
                OperatingSystem = pending.OperatingSystem
            };
        }

        public async Task<bool> ClaimPendingAsync(Guid userId, string code)
        {
            if (!_cache.TryGetValue($"pending-device:{code}", out PendingDevice? pending) || pending is null)
            {
                return false;
            }
            if (pending.ApiKey is not null)
            {
                return false;
            }

            var device = await GetOrCreateDeviceAsync(userId, pending.DeviceName, pending.OperatingSystem);
            pending.ApiKey = device.ApiKey;
            return true;
        }

        public (bool Found, string? ApiKey) PollPending(string code)
        {
            if (!_cache.TryGetValue($"pending-device:{code}", out PendingDevice? pending) || pending is null)
            {
                return (false, null);
            }
            if (pending.ApiKey is null)
            {
                return (true, null);
            }

            _cache.Remove($"pending-device:{code}");
            return (true, pending.ApiKey);
        }
    }
}