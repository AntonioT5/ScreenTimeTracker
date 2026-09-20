using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Service.DTOs.RequestResponse;
using Service.Interface;

namespace Web.Controller
{
    [ApiController]
    [Route("api/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _service;

        public DevicesController(IDeviceService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDeviceRequest request)
        {
            try
            {
                var result = await _service.RegisterDeviceAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("pending")]
        [AllowAnonymous]
        public IActionResult CreatePending([FromBody] PendingDeviceRequest request)
        {
            try
            {
                _service.CreatePending(request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("pending/{code}")]
        [AllowAnonymous]
        public IActionResult PollPending(string code)
        {
            var (found, apiKey) = _service.PollPending(code);

            if (!found)
            {
                return NotFound();
            }
            if (apiKey is null)
            {
                return NoContent();
            }

            return Ok(new { apiKey });
        }

        [HttpGet("pending/{code}/info")]
        [Authorize]
        public IActionResult GetPendingInfo(string code)
        {
            var info = _service.GetPendingInfo(code);
            return info is null ? NotFound() : Ok(info);
        }

        [HttpPost("claim")]
        [Authorize]
        public async Task<IActionResult> Claim([FromBody] ClaimDeviceRequest request)
        {
            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userIdText, out var userId))
            {
                return Unauthorized();
            }

            var ok = await _service.ClaimPendingAsync(userId, request.Code);
            return ok ? NoContent() : NotFound();
        }
    }
}