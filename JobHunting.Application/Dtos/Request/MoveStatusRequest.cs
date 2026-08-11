using JobHunting.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Request
{
    public record MoveStatusRequest(
        ApplicationStatus NewStatus,
        string? Reason
    );
}
