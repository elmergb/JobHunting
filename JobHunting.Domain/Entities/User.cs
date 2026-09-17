using JobHunting.Domain.Primatives;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain.Entities
{
    public class User : AggregateRoot<UserId>
    {
        public string FirstName { get; private set; }
        public string? MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }

        // --- Auth fields ---
        public string PasswordHash { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiry { get; private set; }
        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        // Navigation to profile (same aggregate boundary)
        public UserProfile? Profile { get; private set; }

        private User() { } // EF Core

        public static User Create(
            string firstName,
            string? middleName,
            string lastName,
            string email,
            string passwordHash)
        {
            return new User
            {
                Id = UserId.New(),
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void SetPassword(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public void SetRefreshToken(string token, DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiry = expiry;
        }

        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiry = null;
        }

        public void Deactivate() => IsActive = false;
        public void Activate()   => IsActive = true;
    }
}
