using System;
using Eventuous;

namespace Hotel.Bookings.Domain.Bookings {
    public static class BookingEvents {
        public record RoomBooked(
            string         BookingId,
            string         GuestId,
            string         RoomId,
            DateTimeOffset CheckInDate,
            DateTimeOffset CheckOutDate,
            float          BookingPrice,
            float          PrepaidAmount,
            float          OutstandingAmount,
            string         Currency,
            bool           Paid,
            DateTimeOffset BookingDate
        );

        public record PaymentRecorded(
            string BookingId, float PaidAmount, float Outstanding, string Currency, string PaymentId, string PaidBy, DateTimeOffset PaidAt
        );

        public record DiscountApplied(
            string BookingId, float Discount, float Outstanding, string Currency, string Reason, string AppliedBy, DateTimeOffset AppliedAt
        );

        public static void MapEvents() {
            TypeMap.AddType<RoomBooked>("V1.RoomBooked");
            TypeMap.AddType<PaymentRecorded>("V1.PaymentRecorded");
        }
    }
}