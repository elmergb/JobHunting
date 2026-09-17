using JobHunting.Domain.Primatives;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain.Entities
{
    public class UserProfile : Entity<ProfileId>
    {
        public UserId UserId { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? AvatarUrl { get; private set; }
        public string? Bio { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private UserProfile() { } // EF Core

        public static UserProfile Create(UserId userId)
        {
            return new UserProfile
            {
                Id = ProfileId.New(),
                UserId = userId,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void Update(string? phoneNumber, string? avatarUrl, string? bio)
        {
            PhoneNumber = phoneNumber;
            AvatarUrl = avatarUrl;
            Bio = bio;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
