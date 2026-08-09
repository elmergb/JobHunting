using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace JobHunting.Domain.ValueObjects
{
    public record ApplicationSource
    {
        public SourceType Type { get; init; } // LinkedIn, CompanySite, Referral, Recruiter
        public string? Url { get; init; }
        public string? ReferralContactName { get; init; }

        public static ApplicationSource LinkedIn(string url) =>
            new() { Type = SourceType.LinkedIn, Url = url };

        public static ApplicationSource Referral(string contactName) =>
            new() { Type = SourceType.Referral, ReferralContactName = contactName };
    }
}
