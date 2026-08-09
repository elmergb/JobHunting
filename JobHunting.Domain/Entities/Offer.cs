using JobHunting.Domain.Primatives;
using JobHunting.Domain.ValueObjects;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Domain.Entities
{
    // Domain/Entities/Offer.cs
    public class Offer : Entity<OfferId>
    {
        public ApplicationId ApplicationId { get; private set; }
        public Money BaseSalary { get; private set; }
        public Money? SigningBonus { get; private set; }
        public Money? AnnualBonus { get; private set; }
        public EquityGrant? Equity { get; private set; }
        public string? Benefits { get; private set; }
        public DateTime? Deadline { get; private set; }
        public OfferStatus Status { get; private set; }
        public string? NegotiationNotes { get; private set; }

        private Offer() { }

        public static Offer Create(
                ApplicationId applicationId,
            Money baseSalary,
            DateTime? deadline = null)
        {
            return new Offer
            {
                Id = OfferId.New(),
                ApplicationId = applicationId,
                BaseSalary = baseSalary,
                Deadline = deadline,
                Status = OfferStatus.Pending
            };
        }

        public void Negotiate(Money newBaseSalary, string notes)
        {
            BaseSalary = newBaseSalary;
            NegotiationNotes = notes;
            Status = OfferStatus.Negotiating;
        }

        public void Accept()
        {
            Status = OfferStatus.Accepted;
        }

        public void Decline(string reason)
        {
            Status = OfferStatus.Declined;
            NegotiationNotes = reason;
        }

        public Money TotalCompensation()
        {
            var total = BaseSalary;
            if (AnnualBonus is not null) total += AnnualBonus;
            if (SigningBonus is not null) total += SigningBonus;
            return total;
        }
    }
}
