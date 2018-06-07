using AspNetCoreVideo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace AspNetCoreVideo
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                // tell the builder where to look for files
                .SetBasePath(Directory.GetCurrentDirectory())
                // specify that there is a json file
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        // Note regarding Dependency Injection in ConfigureServices() and Configure():
        // .NET automatically injects some objects for you (IServiceCollection, IApplicationBuilder, and IHostingEnvironment are all examples from this file)
        // Any other objects that need to be injected need to be registered on the IServicesCollection (services) object.

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Breakdown of a few methods used to add services:
            // Singleton: Creates a single instance that is used throughout the application. It creates the instance when the first dependency-injected object is created.
            // Scoped:    Lifetime services. Created once per request within the scope. Equivalent to Singleton in the current scope. In other words, the same instance is reused within the same HTTP request
            // Transient: Created each timem they are requested and won't be reused. Works best for lightweight, stateless services.
            services.AddSingleton(provider => Configuration)
                    .AddSingleton<IMessageService, ConfigurationMessageService>()
                    .AddScoped<IVideoData, MockVideoData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // The IApplicationBuilder object injected here is used to set up the middleware pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMessageService msg)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // The Run middleware component is called at the end of the pipeline, it IS the end of the pipeline (it is a terminal piece of middleware). It is used to process every reponse.
            // No middleware component added after the Run component will execute because Run doesn't call into any other middleware components.
            // The context object contains a Request object which contains everything about the request. It also contains a Response object
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(msg.GetMessage());
            });
        }
    }
}
