﻿namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;

    using Model;

    /// <summary>
    /// Inteface for space type readonly repository.
    /// </summary>
    public interface IMessageDeliveredStatusReadonlyRepository : IReadonlyRepository<MessageDeliveredStatus, Guid>
    {
    }
}
