using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Domain.Entities;
using Repository.Interface;
using Service.DTOs.RequestResponse;
using Service.Interface;

namespace Service.Implementation
{
    public class DeviceService : IDeviceService
    {
        private readonly IRepository<Device> _repository;
        private readonly IRepository<User> _userRepository;

        public DeviceService(IRepository<Device> repository, IRepository<User> userRepository)
        {
            _repository=repository;
            _userRepository=userRepository;
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

            var existingDevice = await _repository.Get(
                selector: d => d,
                predicate: d => d.UserId == user.Id && d.DeviceName == request.DeviceName);

            if (existingDevice is not null)
            {
                return new RegisterDeviceResponse
                {
                    ApiKey = existingDevice.ApiKey,
                    DeviceName = existingDevice.DeviceName,
                };
            }

            var apiKey = GenerateApiKey();

            var device = new Device
            {
                UserId = user.Id,
                DeviceName = request.DeviceName,
                OperatingSystem = request.OperatingSystem,
                ApiKey = apiKey,
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
    }
}