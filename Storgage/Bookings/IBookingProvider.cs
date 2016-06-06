namespace Weezlabs.Storgage.Bookings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Weezlabs.Storgage.DataTransferObjects.Booking;
    using Weezlabs.Storgage.DataTransferObjects.Message;

    public interface IBookingProvider
    {
        /// <summary>
        /// Returns list of chats with approved message offer with information about spaces and etc  and etc
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns></returns>
        List<MyBookingResponse> GetMyBookings(Guid userId);
    }
}
