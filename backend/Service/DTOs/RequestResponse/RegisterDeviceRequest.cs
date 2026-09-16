using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.RequestResponse
{
    public class RegisterDeviceRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
    }
}