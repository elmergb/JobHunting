using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain.ValueObjects
{
    public record Location
    {
        public string City { get; init; }
        public string? State { get; init; }
        public string? Country { get; init; }
        public bool IsRemote { get; init; }

        private Location() { } // EF Core

        public Location(string city, string? state = null, string? country = null, bool isRemote = false)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new Exceptions.DomainException("City cannot be empty");

            City = city;
            State = state;
            Country = country;
            IsRemote = isRemote;
        }
    }
}
