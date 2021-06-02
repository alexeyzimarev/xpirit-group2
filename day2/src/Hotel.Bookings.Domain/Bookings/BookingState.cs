using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Eventuous;
using static Hotel.Bookings.Domain.Bookings.BookingEvents;

namespace Hotel.Bookings.Domain.Bookings {
    public record BookingState : AggregateState<BookingState, BookingId> {
        public string     GuestId     { get; init; }
        public RoomId     RoomId      { get; init; }
        public StayPeriod Period      { get; init; }
        public Money      Price       { get; init; }
        public Money      Outstanding { get; init; }
        public bool       Paid        { get; init; }

        ImmutableList<PaymentRecord>  _paymentRecords  = ImmutableList<PaymentRecord>.Empty;
        ImmutableList<DiscountRecord> _discountRecords = ImmutableList<DiscountRecord>.Empty;

        public IEnumerable<PaymentRecord>  PaymentRecords  => _paymentRecords;
        public IEnumerable<DiscountRecord> DiscountRecords => _discountRecords;

        internal bool HasDiscountBeenAlreadyApplied => !_discountRecords.IsEmpty;

        internal bool HasPaymentBeenRegistered(string paymentId) => _paymentRecords.Any(x => x.PaymentId == paymentId);

        public override BookingState When(object evt) {
            return evt switch {
                V1.RoomBooked booked => this with {
                    Id = new BookingId(booked.BookingId),
                    RoomId = new RoomId(booked.RoomId),
                    Period = new StayPeriod(booked.CheckInDate, booked.CheckOutDate),
                    GuestId = booked.GuestId,
                    Price = new Money {Amount       = booked.BookingPrice, Currency      = booked.Currency},
                    Outstanding = new Money {Amount = booked.OutstandingAmount, Currency = booked.Currency},
                    Paid = booked.Paid
                },
                V1.PaymentRecorded e => this with {
                    _paymentRecords = _paymentRecords.Add(
                        new PaymentRecord(e.PaymentId, new Money {Amount = e.PaidAmount, Currency = e.Currency})
                    ),
                    Outstanding = new Money {Amount = e.Outstanding, Currency = e.Currency},
                },
                V1.DiscountApplied e => this with {
                    _discountRecords =
                    _discountRecords.Add(
                        new DiscountRecord(new Money {Amount = e.Discount, Currency = e.Currency}, e.Reason)
                    ),
                    Outstanding = new Money {Amount = e.Outstanding, Currency = e.Currency},
                },
                V1.BookingFullyPaid => this with {Paid = true},
                _ => this
            };
        }
    }

    public record PaymentRecord(string PaymentId, Money PaidAmount);

    public record DiscountRecord(Money Discount, string Reason);
}