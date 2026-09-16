using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controller
{
    [ApiController]
    [Route("api/summary")]
    [Authorize]
    public class SummaryController : ControllerBase
    {
        private readonly ISummaryService _service;

        public SummaryController(ISummaryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Invalid token.");
            }

            var result = await _service.GetSummaryAsync(userId);
            return Ok(result);
        }
    }
}