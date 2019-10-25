using Autofac;
using Domain0.FastSql;
using Domain0.Nancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Nancy.Owin;
using NLog;

namespace Domain0.Service
{
    public class Startup
    {

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger.json", "Api");
                c.RoutePrefix = "swagger-ui";
            });

            app.UseOwin(x => x.UseNancy(opt =>
            {
                var container = CreateContainer();
                var bootstrapper = new Domain0Bootstrapper(container);
                opt.Bootstrapper = bootstrapper;
            }));
        }

        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(Settings.ConnectionString).Named<string>("connectionString");
            builder.Register(c => LogManager.GetCurrentClassLogger()).As<ILogger>().InstancePerDependency();
            builder.RegisterModule<DatabaseModule>();
            builder.RegisterModule<ApplicationModule>();

            return builder.Build();
        }
    }
}