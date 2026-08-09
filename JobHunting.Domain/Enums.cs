using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain
{
    public enum ApplicationStatus
    {
        Wishlist,       // Saved but not applied
        Applied,        // Application sent
        PhoneScreen,    // Recruiter/HR call
        Technical,      // Coding/system design
        Onsite,         // Final round
        OfferReceived,  // They want you
        Accepted,       // Hired!
        Declined,       // You said no
        Rejected,       // They said no
        Withdrawn       // You pulled out
    }

    public enum InterviewType
    {
        PhoneScreen,
        TechnicalCoding,
        SystemDesign,
        Behavioral,
        TakeHome,
        Onsite,
        FinalRound
    }

    public enum InterviewStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        NoShow
    }

    public enum WorkType
    {
        Remote,
        Hybrid,
        Onsite
    }

    public enum SourceType
    {
        LinkedIn,
        CompanyWebsite,
        Referral,
        Recruiter,
        JobBoard,
        Other
    }

    public enum CompanySize
    {
        Startup,      // 1-50
        Small,        // 51-200
        Medium,       // 201-1000
        Large,        // 1001-10000
        Enterprise    // 10000+
    }

    public enum OfferStatus
    {
        Pending,
        Negotiating,
        Accepted,
        Declined,
        Expired
    }
}
