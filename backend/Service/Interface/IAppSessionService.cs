using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.DTOs.SessionSyncDtos;

namespace Service.Interface
{
    public interface IAppSessionService
    {
        Task<int> IngestBatchAsync(string apiKey, SessionBatchRequest request);
    }
}