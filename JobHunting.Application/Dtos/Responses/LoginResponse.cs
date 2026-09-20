using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Responses
{
    public record LoginResponse(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        string AccessToken,
        DateTime AccessTokenExpiry
    );
}
