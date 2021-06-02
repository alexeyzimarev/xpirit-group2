using System;

namespace Hotel.Bookings.Application.Bookings {
    public static class BookingCommands {
        public record BookRoom(
            string         BookingId,
            string         GuestId,
            string         RoomId,
            DateTimeOffset CheckInDate,
            DateTimeOffset CheckOutDate,
            float          BookingPrice,
            float          PrepaidAmount,
            string         Currency,
            DateTimeOffset BookingDate
        );

        public record RecordPayment(string BookingId, float PaidAmount, string Currency, string PaymentId, string PaidBy);
        
        public record ApplyDiscount(string BookingId, float Discount, string Currency, string Reason, string AppliedBy);
    }
}