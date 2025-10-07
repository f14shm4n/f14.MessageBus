using App.CalcWorker.Application.EventBus.Consumers;
using f14.MessageBus;
using f14.MessageBus.RabbitMQ;
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
                            var opts = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQAppOptions>() ?? throw new InvalidOperationException("RabbitMQ options required. Check you appsettings.json.");
                            config
                                .Connection((cf, cc) =>
                                {
                                    cf.Uri = new Uri(opts.ConnectionString);
                                    cc.RetryPolicy = opts.ConnectionRetryPolicy;
                                })
                                //.PublishEndPoint(ex =>
                                //{
                                //    ex
                                //    .Exchange(opts.CalculatorExchange.Name, ExchangeType.Direct)
                                //    .Queue(opts.CalculatorExchange.Queue)
                                //    .EndPoint(ep => ep.Message<DummyConsumerMessage>());
                                //})
                                //.PublishEndPoint(opts.CalculatorExchange.Name, opts.CalculatorExchange.Queue, endpoint =>
                                //{
                                //    endpoint.Message<DummyConsumerMessage>();
                                //})
                                .ConsumeEndPoint<DummyConsumerMessage>(opts.CalculatorExchange.Name, opts.CalculatorExchange.Queue);
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
