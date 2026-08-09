using System;
using System.Collections.Generic;
using System.Text;
using JobHunting.Domain.Primatives;
using JobHunting.Domain.ValueObjects;


namespace JobHunting.Domain.Entities
{
    // Domain/Entities/Company.cs
    public class Company : AggregateRoot<CompanyId>
    {
        public string Name { get; private set; }
        public string? Industry { get; private set; }
        public CompanySize? Size { get; private set; }
        public string? Website { get; private set; }
        public Location? Location { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Company() { } // EF Core

        public static Company Create(string name, Location? location = null)
        {
            return new Company
            {
                Id = CompanyId.New(),
                Name = name,
                Location = location,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateDetails(string? industry, CompanySize? size, string? website)
        {
            Industry = industry;
            Size = size;
            Website = website;
        }
    }

}
