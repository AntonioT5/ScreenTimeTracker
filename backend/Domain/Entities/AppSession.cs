using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AppSession
    {
        public Guid Id { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Guid DeviceId { get; set; }
        public Device Device { get; set; } = null!;
    }
}