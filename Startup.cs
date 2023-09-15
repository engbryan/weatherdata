using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;
using WeatherData.DataAccess;
using WeatherData.Infrastructure.ExceptionHandling;
using WeatherData.Infrastructure.Logging;
using WeatherData.Models;
using WeatherData.Services;
using WeatherData.Services.Interfaces;
namespace WeatherData
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            services.AddSingleton(Configuration);

            services.AddDbContext<WeatherDataContext>(
                options => options
                    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                );

            services.AddScoped<WeatherDataContext>();
            services.AddScoped<WeatherDataManager>();
            services.AddScoped<AirportStatusManager>();

            services.AddLogging(builder =>
            {
                builder.AddDbLogger();
            });

            services.AddHttpClient();

            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                })
                .AddOData(opt => {
                    opt
                    .Filter()
                    .Select()
                    .Expand()
                    .OrderBy()
                    .SetMaxTop(100)
                    .SkipToken()
                    .Count();

                    opt.AddRouteComponents(
                     "odata",
                     GetEdmModel());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherData", Version = "v1" });

                // Configuração opcional para incluir comentários XML da documentação do código
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddTransient<IWeatherDataService, BrasilAPIWeatherService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseErrorLoggingMiddleware();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherData V1");
                c.RoutePrefix = "swagger";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<WeatherForecast>("WeatherForecast");
            builder.EntitySet<AirportStatus>("AirportStatus");
            builder.EntitySet<DayWeather>("DayWeather");

            builder.EntityType<WeatherForecast>()
                .Function("GetCityWeatherAsOf")
                .Returns<WeatherForecast>();

            builder.EntityType<AirportStatus>()
                .Function("GetAirportStatus")
                .Returns<AirportStatus>();

            return builder.GetEdmModel();
        }
    }
}