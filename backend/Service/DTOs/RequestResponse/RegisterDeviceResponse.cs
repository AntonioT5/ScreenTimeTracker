using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.RequestResponse
{
    public class RegisterDeviceResponse
    {
        public string DeviceName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}