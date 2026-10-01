using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Responses
{
    public record UserProfileResponse(
        string phone_number, string bio, IFormFile avatar
    );
}
