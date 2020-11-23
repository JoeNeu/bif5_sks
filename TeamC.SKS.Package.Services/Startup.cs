using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TeamC.SKS.Package.Services.Filters;
using System.Reflection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using AutoMapper;
using TeamC.SKS.Package.Services.Mapper;
using TeamC.SKS.BusinessLogic.Interfaces;
using TeamC.SKS.BusinessLogic;
using TeamC.SKS.DataAccess.Interfaces;
using TeamC.SKS.DataAccess.Sql;
using Microsoft.EntityFrameworkCore;
using TeamC.SKS.ServiceAgents.Interfaces;
using TeamC.SKS.ServiceAgents;
using System.Net.Http;
using System.Net;

namespace TeamC.SKS.Package.Services
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup class
        /// </summary>
        /// <param name="configuration">Configuration of the application</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration of the application.
        /// </summary>
        /// <value></value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // Add framework services.
            services
            .AddMvc(options =>
            {
                options.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>();
                options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                options.SerializerSettings.Converters.Add(new Helpers.HopJsonConverter());

                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;

            });

            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("1.3.0", new OpenApiInfo
                    {
                        Version = "1.3.0",
                        Title = "Dehael Baket Service fom Johannes und Lisa",
                        Description = Configuration.GetConnectionString("DBTyp"),
                        Contact = new OpenApiContact()
                        {
                            Name = "SKS",
                            Url = new Uri("http://www.technikum-wien.at/"),
                            Email = ""
                        }
                    });

                    c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                    // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                });
            //Automapper 9.0 
            services.AddAutoMapper(typeof(SKSLayerMapper));

            //Businesslogics Transient -- a new instance is provided to every controller and every service -- disposal: never
            services.AddTransient<IWarehouseManagementLogic, WarehouseManagementLogic>();
            services.AddTransient<ILogisticsPartnerLogic, LogisticsPartnerLogic>();
            services.AddTransient<IReceipientLogic, ReceipientLogic>();
            services.AddTransient<ISenderLogic, SenderLogic>();
            services.AddTransient<IStaffLogic, StaffLogic>();

            //DataAccessLayer Scoped -- for every request whithin a defined scope -- disposal: when the scope ends 
            services.AddScoped<IHopRepository, SqlHopRepository>();
            services.AddScoped<IParcelRepository, SqlParcelRepository>();
            services.AddScoped<IWebhookRepository, SqlWebhookRepository>();

            //Datenbank - Entity Framework
            services.AddDbContext<SqlContext>(options => 
            {
                options.UseSqlServer(Configuration.GetConnectionString("DB"),
                                             p => p.UseNetTopologySuite());
            });
            //Service Agents
            services.AddTransient<IGeocoderAgent, MicrosoftBingMapsAgent>();

            //HTTP
            HttpClient httpClient = new HttpClient();
            services.AddSingleton<HttpClient>(httpClient);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRouting();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //TODO: Either use the SwaggerGen generated Swagger contract (generated from C# classes)
                c.SwaggerEndpoint("/swagger/1.3.0/swagger.json", "Parcel Logistics Service");

                //TODO: Or alternatively use the original Swagger contract that's included in the static files
                 //c.SwaggerEndpoint("/swagger-original.json", "Parcel Logistics Service Original");
            });

            // app.UseHttpsRedirection();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //TODO: Enable production exception handling (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
                // app.UseExceptionHandler("/Home/Error");
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
        }
    }
}
