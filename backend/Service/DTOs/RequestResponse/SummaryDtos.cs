using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.RequestResponse
{
    public record AppUsageDto
    {
        public string ProcessName { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
    }

    public record DeviceSummaryDto
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public List<AppUsageDto> Apps { get; set; } = new();
    }

    public record SummaryResponse
    {
        public List<AppUsageDto> Overall { get; set; } = new();
        public List<DeviceSummaryDto> ByDevice { get; set; } = new();
    }
}