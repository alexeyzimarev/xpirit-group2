using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eventuous.Projections.MongoDB;
using Eventuous.Projections.MongoDB.Tools;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using static Hotel.Bookings.Domain.Bookings.BookingEvents;

namespace Hotel.Bookings.Application.Bookings {
    public class MyBookingsProjection : MongoProjection<MyBookings> {
        public MyBookingsProjection(IMongoDatabase database, ILoggerFactory loggerFactory) :
            base(database, ProjectionSubscription.Id, loggerFactory) { }

        protected override ValueTask<Operation<MyBookings>> GetUpdate(object evt) {
            return evt switch {
                V1.RoomBooked e => UpdateOperationTask(
                    e.GuestId,
                    update => update.AddToSet(
                        x => x.Bookings,
                        new MyBookings.Booking(e.BookingId, e.CheckInDate, e.CheckOutDate, e.BookingPrice)
                    )
                ),
                _ => NoOp
            };
        }
    }

    public record MyBookings : ProjectedDocument {
        public MyBookings(string id) : base(id) { }

        public List<Booking> Bookings { get; init; } = new();

        public record Booking(string BookingId, DateTimeOffset CheckInDate, DateTimeOffset CheckOutDate, float Price);
    }
}