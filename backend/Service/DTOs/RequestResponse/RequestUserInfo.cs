using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.RequestResponse
{
    public class RequestUserInfo
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}