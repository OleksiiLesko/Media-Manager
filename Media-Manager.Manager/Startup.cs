using Microsoft.AspNetCore.Builder;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Media_Manager.Manager
{
    /// <summary>
    /// Settings configuration
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Settings of configuration and environment
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }
        /// <summary>
        /// Settings of configuration
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// Settings of environment
        /// </summary>
        public IHostEnvironment Environment { get; }
        /// <summary>
        /// Adding services to the collection of services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddControllersWithViews()
                    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }
        /// <summary>
        /// Responsible for building the application's request processing pipeline.
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
