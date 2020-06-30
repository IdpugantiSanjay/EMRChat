using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMRChat.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EMRChat
{

    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
            => $"{connection.GetHttpContext().Request.Query["practiceId"]}_{connection.GetHttpContext().Request.Query["userId"]}_{connection.GetHttpContext().Request.Query["applicationType"]}";
    }


    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
