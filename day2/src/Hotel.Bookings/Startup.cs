using System.Threading.Tasks;
using Eventuous;
using Eventuous.EventStoreDB;
using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions;
using Hotel.Bookings.Application;
using Hotel.Bookings.Application.Bookings;
using Hotel.Bookings.Domain;
using Hotel.Bookings.Domain.Bookings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;

namespace Hotel.Bookings {
    public class Startup {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            BookingEvents.MapEvents();
            
            // Infrastructure
            services.AddSingleton(
                ConfigureMongo(
                    Configuration["MongoDb:ConnectionString"],
                    Configuration["MongoDb:Database"]
                )
            );
            services.AddEventStoreClient(Configuration["EventStore:ConnectionString"]);
            services.AddSingleton<IEventStore, EsdbEventStore>();
            services.AddSingleton<IAggregateStore, AggregateStore>();
            services.AddSingleton(DefaultEventSerializer.Instance);

            // Application
            services.AddSingleton<BookingsCommandService>();

            // Domain services
            services.AddSingleton<Services.IsRoomAvailable>((id, period) => new ValueTask<bool>(true));
            services.AddSingleton<Services.ConvertCurrency>((from, currency) => new Money(from.Amount * 2, currency));
            
            // Add projections
            services.AddSingleton<ICheckpointStore, MongoCheckpointStore>();
            services
                .AddSubscription<ProjectionSubscription>()
                .AddEventHandler<BookingStateProjection>()
                .AddEventHandler<MyBookingsProjection>();

            // API
            services.AddControllers();

            services.AddSwaggerGen(
                c
                    => c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo {
                            Title = "Event Sourcing", Version = "v0.1"
                        }
                    )
            );
        }

        public void Configure(IApplicationBuilder app) {
            app.UseSwagger();

            app.UseSwaggerUI(
                c => c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Event Sourcing v0.1"
                )
            );
            app.UseDeveloperExceptionPage();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        static IMongoDatabase ConfigureMongo(string connectionString, string database) {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            return new MongoClient(settings).GetDatabase(database);
        }
    }
}
