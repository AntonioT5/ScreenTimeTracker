using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Exceptions;
using Service.Interface;

namespace Web.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly IPredictionService _predictionService;

        public PredictionController(IPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost("generate")]
        [Authorize]
        public async Task<IActionResult> GenerateForCurrentUser()
        {
            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userIdText, out var userId))
            {
                return Unauthorized();
            }
            try
            {
                var prediction = await _predictionService.GeneratePredictionAsync(userId);
                return Ok(prediction);
            }catch(NotEnoughHistoryException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}