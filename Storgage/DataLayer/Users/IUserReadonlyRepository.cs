
namespace Weezlabs.Storgage.DataLayer.Users
{
    using System;
    
    using Model;

    /// <summary>
    /// Interface for readonly repository.
    /// </summary>
    public interface IUserReadonlyRepository : IReadonlyRepository<User, Guid>
    {
    }
}
