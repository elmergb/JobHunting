using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Request
{
    public record CreateUserProfile(
        string phone_number, string bio, IFormFile avatar
    );
}
