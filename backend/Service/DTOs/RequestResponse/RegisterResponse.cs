using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.RequestResponse
{
    public record RegisterResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}