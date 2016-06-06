namespace Weezlabs.Storgage.DataLayer.Spaces
{
    using System;

    using Model;

    /// <summary>
    /// Interface for space repository.
    /// </summary>
    public interface ISpaceReadonlyRepository : IReadonlyRepository<Space, Guid>
    {
    }
}
