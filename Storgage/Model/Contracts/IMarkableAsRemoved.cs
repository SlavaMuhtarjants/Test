namespace Weezlabs.Storgage.Model.Contracts
{
    using System;

    /// <summary>
    /// Interface for data objects that are not removed physically.
    /// </summary>
    public interface IMarkableAsRemoved
    {
        /// <summary>
        /// If true then object was marked as removed.
        /// </summary>
        Boolean WasRemoved { get; set; }
    }
}
