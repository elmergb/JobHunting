using JobHunting.Application.Common;
using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Dtos.Responses;
using JobHunting.Application.Services.Interface;
using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using JobHunting.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using UserId = JobHunting.Domain.Primatives.UserId;

namespace JobHunting.Application.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
                return Result<UserResponse>.Failure(Error.Invalid("FirstName is required"));

            if (string.IsNullOrWhiteSpace(request.LastName))
                return Result<UserResponse>.Failure(Error.Invalid("LastName is required"));

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<UserResponse>.Failure(Error.Invalid("Email is required"));

            if (string.IsNullOrWhiteSpace(request.Password))
                return Result<UserResponse>.Failure(Error.Invalid("Password is required"));

            if (request.Password.Length < 8)
                return Result<UserResponse>.Failure(Error.Invalid("Password must be at least 8 characters"));

            var existing = await _userRepository.GetByEmailAsync(request.Email, ct);
            Console.WriteLine(existing.FirstName);

            if (existing is not null)
                return Result<UserResponse>.Failure(Error.Conflict("Email is already in use"));

            var passwordHash = _passwordHasher.HashPassword(null!, request.Password);

            var user = User.Create(
                firstName: request.FirstName,
                middleName: request.MiddleName,
                lastName: request.LastName,
                email: request.Email,
                passwordHash: passwordHash
            );

            await _userRepository.AddAsync(user, ct);

            return Result<UserResponse>.Success(MapToResponse(user));
        }

        public async Task<Result<UserResponse>> GetByIdAsync(Guid userId, CancellationToken ct = default)
        {
            var id = new UserId(userId);
            var user = await _userRepository.GetByIdAsync(id, ct);
            Console.WriteLine(user?.FirstName);
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
    }
}
