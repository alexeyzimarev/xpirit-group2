using Eventuous;
using Hotel.Payments.Domain;

namespace Hotel.Payments.Application {
    public class CommandService : ApplicationService<Payment, PaymentState, PaymentId> {
        public CommandService(IAggregateStore store) : base(store) {
            OnNew<PaymentCommands.RecordPayment>(
                cmd => new PaymentId(cmd.PaymentId),
                (payment, cmd) => payment.ProcessPayment(
                    new PaymentId(cmd.PaymentId),
                    cmd.BookingId,
                    cmd.Amount,
                    cmd.Method, 
                    cmd.Provider
                )
            );
        }
    }

    public static class PaymentCommands {
        public record RecordPayment(string PaymentId, string BookingId, float Amount, string Method, string Provider);
    }
}