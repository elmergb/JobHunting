using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Request
{
    public record CreateUserRequest(
        string FirstName,
        string? MiddleName,
        string LastName,
        string Email,
        string Password
    );
}
