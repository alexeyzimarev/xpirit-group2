using System;
using System.Threading.Tasks;
using Eventuous.Projections.MongoDB;
using Eventuous.Projections.MongoDB.Tools;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using static Hotel.Bookings.Domain.Bookings.BookingEvents;

namespace Hotel.Bookings.Application.Bookings {
    public class BookingStateProjection : MongoProjection<BookingDocument> {
        public BookingStateProjection(IMongoDatabase database, ILoggerFactory loggerFactory)
            : base(database, ProjectionSubscription.Id, loggerFactory) { }

        protected override ValueTask<Operation<BookingDocument>> GetUpdate(object evt) {
            return evt switch {
                V1.RoomBooked e => UpdateOperationTask(
                    e.BookingId,
                    update => update
                        .SetOnInsert(x => x.Id, e.BookingId)
                        .Set(x => x.GuestId, e.GuestId)
                        .Set(x => x.RoomId, e.RoomId)
                        .Set(x => x.CheckInDate, e.CheckInDate)
                        .Set(x => x.CheckOutDate, e.CheckOutDate)
                        .Set(x => x.BookingPrice, e.BookingPrice)
                        .Set(x => x.Outstanding, e.OutstandingAmount)
                        .Set(x => x.Paid, e.Paid)
                ),
                _ => NoOp
            };
        }
    }

    public record BookingDocument : ProjectedDocument {
        public BookingDocument(string id) : base(id) { }

        public string         GuestId      { get; init; }
        public string         RoomId       { get; init; }
        public DateTimeOffset CheckInDate  { get; init; }
        public DateTimeOffset CheckOutDate { get; init; }
        public float          BookingPrice { get; init; }
        public float          PaidAmount   { get; init; }
        public float          Outstanding  { get; init; }
        public bool           Paid         { get; init; }
    }
}