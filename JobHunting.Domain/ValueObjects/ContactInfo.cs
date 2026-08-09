using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain.ValueObjects
{
    public record ContactInfo
    {
        public string Name { get; init; }
        public string? Role { get; init; }
        public string? Email { get; init; }
    }
}
