namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;

    using Model;

    /// <summary>
    /// Interface for email verification status repository.
    /// </summary>
    public interface IEmailVerificationStatusReadonlyRepository : IReadonlyRepository<EmailVerificationStatus, Guid>
    {
    }
}
