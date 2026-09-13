using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.SessionSyncDtos;
using Service.Interface;

namespace Web.Controller
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly IAppSessionService _appSessionService;

        public SessionsController(IAppSessionService appSessionService)
        {
            _appSessionService = appSessionService;
        }

        [HttpPost("batch")]
        public async Task<IActionResult> IngestBatch([FromBody] SessionBatchRequest request)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return Unauthorized("Missing Authorization header.");
            }

            var apiKey = authHeader.ToString().Replace("Bearer ", "");

            try
            {
                var insertedCount = await _appSessionService.IngestBatchAsync(apiKey, request);
                return Ok(new { inserted = insertedCount });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid API key.");
            }
        }
    }
}