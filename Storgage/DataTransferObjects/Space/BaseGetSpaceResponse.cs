namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;    

    /// <summary>
    /// Contains information about space.
    /// </summary>
    abstract public class  BaseGetSpaceResponse : SpaceInfo
    {
        /// <summary>
        /// Space identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Links to space photo.
        /// </summary>
        public IEnumerable<Photo> Photo { get; set; }

        /// <summary>
        /// Available Since date.
        /// </summary>
        public DateTimeOffset? AvailableSince { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseGetSpaceResponse()
        {
        }

        /// <summary>
        /// Create Dto from model.
        /// </summary>
        /// <param name="space">Model object.</param>
        public BaseGetSpaceResponse(Model.Space space)
            : base(space)
        {
            Contract.Requires(space != null);
            Contract.Requires(space.PhotoLibraries != null);

            Id = space.Id;
            Photo = space.PhotoLibraries.OrderBy(x => x.Id).Select(x => new Photo(x));
            AvailableSince = space.AvailableSince;           
        }
    }
}