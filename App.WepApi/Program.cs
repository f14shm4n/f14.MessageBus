using App.Constants;
using Shared.EventBus;
using Shared.RabbitMQ;

namespace App.WepApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEventBus(c =>
            {
                //c.AddConsumer()                
                c.UseEventBus<RabbitMQBusBuilder>(rb =>
                {
                    rb
                    .CreateConnection(new Uri(builder.Configuration.GetValue("RabbitMQ:ConnectionString", RabbitMQDefaults.DefaultUri)))
                    .SetupExchange(
                        exchange: builder.Configuration.GetValue("RabbitMQ:CalculatorExchange:name", AppConstants.CalculatorExchangeName),
                        type: builder.Configuration.GetValue("RabbitMQ:CalculatorExchange:type", AppConstants.CalculatorExchangeType),
                        durable: builder.Configuration.GetValue("RabbitMQ:CalculatorExchange:isDurable", AppConstants.CalculatorExchangeIsDurable)
                        );

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
