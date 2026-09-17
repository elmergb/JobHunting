using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Responses
{
    public record UserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        DateTime CreatedAt
    );
}
