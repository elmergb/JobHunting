using JobHunting.Application.Dtos.Request;
using JobHunting.Application.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Services.Interface
{
    public interface IUserProfile
    {
        Task<UserProfileResponse> CreateAsync(CreateUserProfile user, CancellationToken ct =default);

    }
}
