namespace Hotel.Payments.Domain {
    public static class PaymentEvents {
        public record PaymentRecorded(string PaymentId, float Amount);
    }
}