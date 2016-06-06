namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;

    using Model;

    /// <summary>
    /// Interface for phone verification status repository.
    /// </summary>
    public interface IPhoneVerificationStatusReadonlyRepository : IReadonlyRepository<PhoneVerificationStatus, Guid>
    {
    }
}
