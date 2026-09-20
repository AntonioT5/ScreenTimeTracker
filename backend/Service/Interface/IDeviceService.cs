using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.DTOs.RequestResponse;

namespace Service.Interface
{
    public interface IDeviceService
    {
        Task<RegisterDeviceResponse> RegisterDeviceAsync(RegisterDeviceRequest request);
        void CreatePending(PendingDeviceRequest request);
        PendingDeviceInfo? GetPendingInfo(string code);
        Task<bool> ClaimPendingAsync(Guid userId, string code);
        (bool Found, string? ApiKey) PollPending(string code);
        Task<bool> IsKeyAwaizble(string ApiKey);

        Task UnLinkDevice(Guid userId, string DeviceName);
    }
}