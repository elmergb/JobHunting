using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain.Primatives
{
    public record CompanyId(Guid Value)
    {
        public static CompanyId New() => new(Guid.NewGuid());
    }

    public record ApplicationId(Guid Value)
    {
        public static ApplicationId New() => new(Guid.NewGuid());
    }

    public record InterviewId(Guid Value)
    {
        public static InterviewId New() => new(Guid.NewGuid());
    }

    public record OfferId(Guid Value)
    {
        public static OfferId New() => new(Guid.NewGuid());
    }

    public record DocumentId(Guid Value)
    {
        public static DocumentId New() => new(Guid.NewGuid());
    }
}
