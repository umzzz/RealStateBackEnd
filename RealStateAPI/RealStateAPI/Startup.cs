using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealStateAPI.Models;

namespace RealStateAPI
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
            // using DI to get the connection string and regestering a singelton for the life of the application
            services.Configure<UserDataBaseSettings>(Configuration.GetSection(nameof(UserDataBaseSettings)));

            services.AddSingleton<IUserDataBaseSettings>(sp => sp.GetRequiredService<IOptions<UserDataBaseSettings>>().Value);

            services.Configure<ListingDataBaseSettings>(Configuration.GetSection(nameof(ListingDataBaseSettings)));

            services.AddSingleton<IListingDataBaseSettings>(sp => sp.GetRequiredService<IOptions<ListingDataBaseSettings>>().Value);

            services.Configure<ListingInquiryDataBaseModel>(Configuration.GetSection(nameof(ListingInquiryDataBaseModel)));

            services.AddSingleton<IListingInquiryDataBaseModel>(sp => sp.GetRequiredService<IOptions<ListingInquiryDataBaseModel>>().Value);


            services.AddControllers();
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
