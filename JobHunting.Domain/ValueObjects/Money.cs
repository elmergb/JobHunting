using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace JobHunting.Domain.ValueObjects
{
    // Domain/ValueObjects/Money.cs
    public record Money
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "PHP";

        private Money() { } // EF Core protected constructor

        public Money(decimal amount, string currency = "PHP")
        {
            if (amount < 0) throw new DomainException("Salary cannot be negative");
            Amount = amount;
            Currency = currency;
        }

        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency) throw new DomainException("Currency mismatch");
            return new Money(a.Amount + b.Amount, a.Currency);
        }
    }

    // Domain/ValueObjects/ApplicationSource.cs


    // Domain/ValueObjects/ContactInfo.cs

}
