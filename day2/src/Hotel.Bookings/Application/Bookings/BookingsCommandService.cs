using System;
using System.Threading.Tasks;
using Eventuous;
using Hotel.Bookings.Domain;
using Hotel.Bookings.Domain.Bookings;
using static Hotel.Bookings.Application.Bookings.BookingCommands;

namespace Hotel.Bookings.Application.Bookings {
    public class BookingsCommandService : ApplicationService<Booking, BookingState, BookingId> {
        public BookingsCommandService(IAggregateStore store, Services.IsRoomAvailable isRoomAvailable) : base(store) {
            OnNewAsync<BookRoom>(
                cmd => new BookingId(cmd.BookingId),
                (booking, cmd, _) => booking.BookRoom(
                    new BookingId(cmd.BookingId),
                    cmd.GuestId,
                    new RoomId(cmd.RoomId),
                    new StayPeriod(cmd.CheckInDate, cmd.CheckOutDate),
                    new Money(cmd.BookingPrice, cmd.Currency),
                    new Money(cmd.PrepaidAmount, cmd.Currency),
                    DateTimeOffset.Now,
                    isRoomAvailable
                )
            );

            OnExisting<RecordPayment>(
                cmd => new BookingId(cmd.BookingId),
                (booking, cmd) => booking.RecordPayment(
                    new Money(cmd.PaidAmount, cmd.Currency),
                    cmd.PaymentId,
                    cmd.PaidBy,
                    DateTimeOffset.Now
                )
            );

            OnExisting<BookingEvents.DiscountApplied>(
                cmd => new BookingId(cmd.BookingId),
                (booking, cmd) => booking.ApplyDiscount(
                    new Money(cmd.Discount, cmd.Currency),
                    cmd.Reason,
                    cmd.AppliedBy,
                    DateTimeOffset.Now
                )
            );
        }
    }
}