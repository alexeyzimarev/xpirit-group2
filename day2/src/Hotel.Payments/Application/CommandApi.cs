using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static Hotel.Payments.Application.PaymentCommands;

namespace Hotel.Payments.Application {
    [Route("payment")]
    public class CommandApi : ControllerBase {
        readonly CommandService _service;
        
        public CommandApi(CommandService service) => _service = service;

        [HttpPost]
        public Task RegisterPayment([FromBody] RecordPayment cmd, CancellationToken cancellationToken)
            => _service.Handle(cmd, cancellationToken);
    }
}