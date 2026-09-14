using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Repository.Interface;
using Service.DTOs;
using Service.DTOs.RequestResponse;
using Service.Interface;

namespace Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _repository;
        private readonly ITokenService _tokenService;

        public AuthService(IRepository<User> repository, ITokenService tokenService)
        {
            _repository=repository;
            _tokenService=tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _repository.Get(
                selector: u => u,
                predicate: u=> u.Username==request.Username
            );

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var token = _tokenService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if (request.Password != request.RepeatPassword)
            {
                throw new InvalidOperationException("Passwords do not match.");
            }

            var existingUser = await _repository.Get(
                selector: u => u,
                predicate: u => u.Username == request.Username || u.Email == request.Email);

            if (existingUser is not null)
            {
                throw new InvalidOperationException("Username or email is already taken.");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
            };

            await _repository.InsertAsync(user);

            return new RegisterResponse
            {
                UserId = user.Id,
                Username = user.Username,
            };
        }
    }
}