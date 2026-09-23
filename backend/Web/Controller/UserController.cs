using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.RequestResponse;
using Service.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace Web.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService service)
        {
            _userService=service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeUserInfo([FromBody] RequestUserInfo request)
        {
            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userIdText, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                await _userService.ChangeUserData(userId, request.Username, request.Email);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            
        }
    }
}