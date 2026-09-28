using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.DTOs.RequestResponse
{
    public class PredictionResponse
    {
        public int TotalScreenTime { get; set; }
        public string MostUsedApp { get; set; } = string.Empty;
    }
}