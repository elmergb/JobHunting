using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain.Primatives
{
    public interface IBaseEntity<TId> where TId : notnull
    {
        TId Id { get; }
    }
}
