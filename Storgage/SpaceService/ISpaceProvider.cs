namespace Weezlabs.Storgage.SpaceService
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using DataTransferObjects.Space;
    using Weezlabs.Storgage.DataTransferObjects.Message;

    public interface ISpaceProvider
    {
        /// <summary>
        /// Space provider.
        /// </summary>
        /// <param name="userId">Returns spaces by user</param>
        /// <returns>Searched spaces.</returns>
        List<GetSpaceResponseForOwner> GetSpaces(Guid userId);

        /// <summary>
        /// Upload photos
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="formData">IEnumerable of files.</param>
        /// <returns>Enumerable of uploaded photos.</returns>
        IEnumerable<Photo> UploadPhotos(Guid spaceId, Guid actorId, IEnumerable<HttpContent> formData);

        /// <summary>
        /// Delete photos by identifiers
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="photoIds">List of photo identifiers to delete.</param>
        void DeletePhotos(Guid spaceId, Guid actorId, List<Guid> photoIds);

            /// <summary>
        /// Post new Space for user
        /// </summary>
        /// <param name="userId">Link to user that will be owner of this space</param>
        /// <param name="space">SpaceInfo that will be added</param>
        /// <returns>Space with AdSpace, SpaceAddress and User</returns>
        GetSpaceResponseForOwner PostSpace(Guid userId, PublishSpaceRequest space);
        
        /// <summary>
        /// Deletes single Space by ID
        /// It doesn't deleted related entities
        /// It disable space in the User's space list
        /// </summary>
        /// <param name="Id">Space of Identifier</param>
        /// <param name="actorId">Actor who wants to delete space.</param>
        void Delete(Guid Id, Guid actorId);

        /// <summary>
        /// Returns info about space.
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Space info.</returns>
        GetSpaceResponseForOwner GetSpace(Guid spaceId, Guid actorId);

        /// <summary>
        /// Returns public info about space.
        /// </summary>
        /// <param name="spaceId">Space identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Space  info.</returns>
        GetSpaceResponse GetSpace(Guid spaceId);

        /// <summary>
        /// Full Space update
        /// </summary>
        /// <param name="spaceId">Space identifier</param>
        /// <param name="spaceToUpdate">New Space attributes that will be set</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Curret space with new attributes</returns>
        GetSpaceResponseForOwner Update(Guid spaceId, EditSpaceRequest spaceToUpdate, Guid actorId);

        /// <summary>
        /// Retuns forecasted rate by size type and zipCode
        /// </summary>
        /// <param name="sizeType">space size type</param>
        /// <param name="zipCode">space zip code</param>
        /// <returns>forecased rate value</returns>
        decimal GetForecastedRate(Model.Enums.SizeType sizeType, String zipCode);
    }
}
