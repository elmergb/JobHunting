using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Request
{
    public record LoginRequest(
        string Email,
        string Password
    );
}
