namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;

    using Model;

    /// <summary>
    /// Inteface for PaymentPointType readonly repository.
    /// </summary>
    public interface IPaymentPointTypeReadonlyRepository : IReadonlyRepository<PaymentPointType, Guid>
    {
    }
}
