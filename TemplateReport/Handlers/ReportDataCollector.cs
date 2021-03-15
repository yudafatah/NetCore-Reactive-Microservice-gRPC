using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template.DTO;
using TemplateReport.Services;

namespace TemplateReport.Handlers
{
    public class ReportDataCollector : IHostedService
    {
        private const int DEFAULT_QUANTITY = 100;

        private readonly ISubscriber subscriber;
        private readonly IMemoryReportStorage memoryReportStorage;

        public ReportDataCollector(ISubscriber subscriber, IMemoryReportStorage memoryReportStorage)
        {
            this.subscriber = subscriber;
            this.memoryReportStorage = memoryReportStorage;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(ProcessMessage);
            return Task.CompletedTask;
        }

        private bool ProcessMessage(string message, IDictionary<string, object> headers)
        {
            if (message.Contains("Product"))
            {

            }
            else
            {
                var order = JsonConvert.DeserializeObject<OrderDetail>(message);
                if (memoryReportStorage.Get().Any(x => x.ProductName.Equals(order.Name)))
                {
                    memoryReportStorage.Get().First(x => x.ProductName.Equals(order.Name)).Count -= order.Quantity;
                }
                else
                {
                    memoryReportStorage.Add(new DTO.Report
                    {
                        ProductName = order.Name,
                        Count = DEFAULT_QUANTITY - order.Quantity
                    });
                }
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
