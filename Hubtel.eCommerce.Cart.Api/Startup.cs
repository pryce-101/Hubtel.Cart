using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Repo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;
using Sentry.AspNetCore;

namespace Hubtel.eCommerce.Cart.Api
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
            services.AddCors();

            services.AddTransient<IShoppingCart, ShoppingCartRepo>();
            services.Configure<HtmlHelperOptions>(o => o.ClientValidationEnabled = true);
            
            services.AddEntityFrameworkSqlServer();
            //services.AddDbContext<ShoppingCartDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ShoppingCartDBContext>(options=> options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1" , new OpenApiInfo {Title = "Hubtel.eCommerce.Cart Api", Version="v1" });
            
            });
            var swaggerOption = new SwaggerUIOptions
            {
                RoutePrefix = string.Empty
            };
          
            services.AddSingleton(swaggerOption);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c=> {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hubtel.eCommerce.Cart Api");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
              builder.WithOrigins()
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
          );
            app.UseHttpsRedirection();
            app.UseSentryTracing();
            app.UseMvc();
            
        }
    }
}
