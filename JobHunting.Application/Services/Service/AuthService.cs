using JobHunting.Application.Common;
using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Dtos.Responses;
using JobHunting.Application.Services.Interface;
using JobHunting.Application.Settings;
using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using JobHunting.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserId = JobHunting.Domain.Primatives.UserId;

namespace JobHunting.Application.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtSettings _jwtSettings;
        private readonly ICurrentUserService _currentUser;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IOptions<JwtSettings> jwtSettings,
            ICurrentUserService currentUser)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtSettings.Value;
            _currentUser = currentUser;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<LoginResponse>.Failure(Error.Invalid("Email is required"));

            if (string.IsNullOrWhiteSpace(request.Password))
                return Result<LoginResponse>.Failure(Error.Invalid("Password is required"));

            var user = await _userRepository.GetByEmailAsync(request.Email, ct);

            if (user is null)
                return Result<LoginResponse>.Failure(Error.Unauthorized("Invalid email or password"));

            if (!user.IsActive)
                return Result<LoginResponse>.Failure(Error.Forbidden("Account is deactivated"));

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                return Result<LoginResponse>.Failure(Error.Unauthorized("Invalid email or password"));

            var accessToken = GenerateAccessToken(user);
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            user.SetRefreshToken(refreshToken, refreshTokenExpiry);
            await _userRepository.UpdateAsync(user, ct);

            return Result<LoginResponse>.Success(new LoginResponse(
                UserId: user.Id.Value,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                AccessToken: accessToken,
                AccessTokenExpiry: accessTokenExpiry
            ));
        }


        public async Task<Result<UserResponse>> GetCurrentUser(CancellationToken ct = default)
        {
            var rawId = _currentUser.UserId;

            if (string.IsNullOrWhiteSpace(rawId))
                return Result<UserResponse>.Failure(Error.Unauthorized("Not authenticated"));

            if (!Guid.TryParse(rawId, out var guid))
                return Result<UserResponse>.Failure(Error.Invalid("Invalid user identifier in token"));

            var currentId = new UserId(guid);

            var user = await _userRepository.GetByIdAsync(currentId, ct);

            if (user is null)
                return Result<UserResponse>.Failure(Error.NotFound("User not found"));

            return Result<UserResponse>.Success(MapToResponse(user));
        }

        private static UserResponse MapToResponse(User user) =>
            new(
                Id: user.Id.Value,
                FirstName: user.FirstName,
                MiddleName: user.MiddleName,
                LastName: user.LastName,
                Email: user.Email,
                CreatedAt: user.CreatedAt
            );

        private string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.Value.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer:             _jwtSettings.Issuer,
                audience:           _jwtSettings.Audience,
                claims:             claims,
                expires:            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}
