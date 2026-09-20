using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Services.Interface
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
    }
}
