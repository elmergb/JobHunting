using System;
using System.Collections.Generic;
using System.Text;
using JobHunting.Domain.Exceptions;

namespace JobHunting.Domain.ValueObjects
{
    public record EquityGrant
    {
        public int Shares { get; init; }
        public decimal StrikePrice { get; init; }
        public decimal? Current409AValuation { get; init; }
        public int VestingYears { get; init; }
        public int CliffMonths { get; init; }

        private EquityGrant() { }

        public EquityGrant(int shares, decimal strikePrice, int vestingYears = 4, int cliffMonths = 12, decimal? current409AValuation = null)
        {
            if (shares <= 0) throw new DomainException("Shares must be positive");
            if (vestingYears <= 0) throw new DomainException("Vesting years must be positive");

            Shares = shares;
            StrikePrice = strikePrice;
            VestingYears = vestingYears;
            CliffMonths = cliffMonths;
            Current409AValuation = current409AValuation;
        }

        public decimal EstimatedValueAt(decimal futureSharePrice)
        {
            return Shares * (futureSharePrice - StrikePrice);
        }
    }
}
