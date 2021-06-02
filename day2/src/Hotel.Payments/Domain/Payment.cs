using Eventuous;

namespace Hotel.Payments.Domain {
    public class Payment : Aggregate<PaymentState, PaymentId> {
        public void ProcessPayment(string paymentId, float amount) {
            Apply(new PaymentEvents.PaymentRecorded(paymentId, amount));
        }
    }

    public record PaymentState : AggregateState<PaymentState, PaymentId> {
        
    }

    public record PaymentId(string Value) : AggregateId(Value);
}