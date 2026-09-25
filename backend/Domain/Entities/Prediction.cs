using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class Prediction : BaseEntity
    {
        public DateOnly PredictionForDate { get; set; }
        public int PredictedTotalSeconds { get; set; }
        public string PredictedTopApp { get; set; } = string.Empty;
        public DateTime GeneretedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}