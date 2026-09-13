using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.SessionSyncDtos
{
    public class SessionDto
    {
        public string ProcessName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class SessionBatchRequest
    {
        public List<SessionDto> Sessions { get; set; } = new();
    }
}