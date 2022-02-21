using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PickPoint.Core.Contracts;
using PickPoint.Core.Services;
using PickPoint.Core.Validators;
using PickPoint.Data;

namespace PickPoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.WriteIndented = true;
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

            services.AddDbContext<PickPointContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSql")), ServiceLifetime.Transient);

            services.AddSingleton<IOrderValidator, OrderItemsCountValidator>();
            services.AddSingleton<IOrderValidator, DeliveryPointNumberValidator>();
            services.AddSingleton<IOrderValidator, PhoneNumberValidator>();

            services.AddSingleton(provider => new List<IOrderValidator>
            {
                provider.GetService<OrderItemsCountValidator>(),
                provider.GetService<DeliveryPointNumberValidator>(),
                provider.GetService<PhoneNumberValidator>()
            });

            services.AddScoped(s => new OrderRepository(s.GetService<PickPointContext>()));
            services.AddScoped(s => new DeliveryPointRepository(s.GetService<PickPointContext>()));
            services.AddScoped<IDeliveryService, DeliveryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
