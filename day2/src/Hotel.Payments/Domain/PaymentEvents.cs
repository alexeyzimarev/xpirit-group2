using Eventuous;

namespace Hotel.Payments.Domain {
    public static class PaymentEvents {
        public record PaymentRecorded(string PaymentId, string BookingId, float Amount, string Method, string Provider);

        public static void MapEvents() {
            TypeMap.AddType<PaymentRecorded>("PaymentRecorded");
        }
    }
}