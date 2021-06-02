using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eventuous;
using Eventuous.EventStoreDB;
using Hotel.Payments.Application;
using Hotel.Payments.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Hotel.Payments {
    public class Startup {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            PaymentEvents.MapEvents();
            
            services.AddEventStoreClient(Configuration["EventStore:ConnectionString"]);
            services.AddSingleton<IEventStore, EsdbEventStore>();
            services.AddSingleton<IAggregateStore, AggregateStore>();
            services.AddSingleton(DefaultEventSerializer.Instance);

            services.AddSingleton<CommandService>();
            
            services.AddControllers();

            services.AddSwaggerGen(
                c => c.SwaggerDoc("v1", new OpenApiInfo {Title = "Hotel.Payments", Version = "v1"})
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel.Payments v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}