using App.CalcWorker.Application.EventBus.Consumers;
using Shared.EventBus;
using Shared.RabbitMQ;
using Shared.RabbitMQ.App;

namespace App.CalcWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddEventBus(setup =>
                {
                    setup
                        .Consume<DummyConsumerMessage, DummyConsumer>()
                        .UseEventBus<RabbitMQBusConfigurer>(config =>
                        {
                            var opts = builder.Configuration.Get<RabbitMQAppOptions>() ?? throw new InvalidOperationException("RabbitMQ options required. Check you appsettings.json.");
                            config
                                .ConfigureConnection((cf, cc) =>
                                {
                                    cf.Uri = new Uri(opts.ConnectionString);
                                    cc.RetryPolicy = opts.ConnectionRetryPolicy;
                                })
                                .ConfigureEndPoint(opts.CalculatorExchange.Name, opts.CalculatorExchange.Queue, endpoint =>
                                {
                                    endpoint.Consume<DummyConsumerMessage>();
                                });
                        });
                });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
