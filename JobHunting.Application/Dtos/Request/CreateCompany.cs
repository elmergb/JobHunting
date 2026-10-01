using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Application.Dtos.Request
{
    public class CompanyRequest
    {
        public string Name { get; set; } = null!;
        public string? Industry { get; set; }
        public string? Size { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
    }
}
