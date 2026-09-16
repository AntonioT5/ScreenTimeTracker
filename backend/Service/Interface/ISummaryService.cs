using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.DTOs.RequestResponse;

namespace Service.Interface
{
    public interface ISummaryService
    {
        Task<SummaryResponse> GetSummaryAsync(Guid userId);
    }
}