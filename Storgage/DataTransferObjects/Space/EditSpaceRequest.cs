namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;

    /// <summary>
    /// Contains data about request to edit space
    /// </summary>
    public class EditSpaceRequest : PublishSpaceRequest
    {
        /// <summary>
        /// Identifier of space.
        /// </summary>
        public Guid Id { get; set; }

        public EditSpaceRequest()
            : base()
        {

        }

    }
}