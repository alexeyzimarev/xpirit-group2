using Eventuous;
using Hotel.Payments.Domain;

namespace Hotel.Payments.Application {
    public class CommandService : ApplicationService<Payment, PaymentState, PaymentId> {
        public CommandService(IAggregateStore store) : base(store) {
            OnNew<PaymentCommands.RecordPayment>(
                cmd => new PaymentId(cmd.PaymentId),
                (payment, cmd) => payment.ProcessPayment(cmd.PaymentId, cmd.Amount)
            );
        }
    }

    static class PaymentCommands {
        public record RecordPayment(string PaymentId, float Amount);
    }
}