namespace Weezlabs.Storgage.DataLayer.Ratings
{    
    using System;
    using System.Linq;

    using Model;

    /// <summary>
    /// Interface for rating repository.
    /// </summary>
    public interface IRatingRepository : IRepository<Rating, Guid>
    {
    }
}
