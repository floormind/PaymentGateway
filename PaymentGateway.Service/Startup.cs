using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PaymentGateway.Repository.Classes;
using PaymentGateway.Repository.Interfaces;
using PaymentGateway.Repository.Models.Classes;
using PaymentGateway.Repository.Models.Interfaces;
using PaymentGateway.Service.Helper.Classes;
using PaymentGateway.Service.Helper.Interfaces;
using PaymentGateway.Service.Models.Classes;
using PaymentGateway.Service.Models.Interfaces;
using Stripe;

namespace PaymentGateway.Service
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
            // We are going to assume that this is running on the Development environment
            // The tokens may be different for production
            services.Configure<Models.Classes.BraintreeBraintreePaymentGatewayConfiguration>(Configuration.GetSection("BraintreeGateway:Development"));
            services.Configure<Models.Classes.StripePaymentGatewayConfiguration>(Configuration.GetSection("StripeGateway:Development"));

            services.AddSingleton<IBraintreePaymentGatewayConfiguration>(sp =>
                sp.GetRequiredService<IOptions<Models.Classes.BraintreeBraintreePaymentGatewayConfiguration>>().Value);

            services.AddSingleton<IStripePaymentGatewayConfiguration>(sp =>
                sp.GetRequiredService<IOptions<StripePaymentGatewayConfiguration>>().Value);
            
            //services.AddScoped<IPaymentGatewayAdapter<Transaction>, BraintreePaymentAdapter<Transaction>>(); // USE FOR BRAINTREE CHARGE
            services.AddScoped<IPaymentGatewayAdapter<Charge>, StripePayementAdapter<Charge>>(); // USE FOR STRIPE CHARGE
            
            services.AddScoped<IBraintreeGateway, BraintreeGateway>();
            services.AddScoped<IRepository, DummyRepository>();
            services.AddScoped<IList<PaymentHistory>, List<PaymentHistory>>();
            services.AddTransient<IPaymentRecord, InternalPaymentRecord>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway.Service v1"));
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
