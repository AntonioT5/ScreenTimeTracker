using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DailySummary
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}