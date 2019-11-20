using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sys.OA.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {            //services.AddTransient<Gc.CoreSys.Service.Interfaces.IConsultService>();
            Configuration = configuration;
            this.Env = env;
        }

        public IHostingEnvironment Env { get; }
        public IConfiguration Configuration { get; }
        public string[] docs = new[] { "未分类" };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<Sys.OA.Service.Implements.UserInfoService>();
            //注册Swagger生成器，定义一个和多个Swagger 文档
            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "NumberOne's API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "乐观的student",
                        Email = "2543600431@qq.com",
                        Url = "https://home.cnblogs.com/u/cn-littlestrive/"
                    },
                    License = new License
                    {
                        Name = "NumberOne",
                        Url = "https://github.com/veryCoolFirst"
                    }

                });
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "Sys.OA.API.xml");
                c.IncludeXmlComments(xmlPath);
            });
            //if (Env.IsDevelopment())
            //{
            //    services.AddSwaggerGen(options =>
            //    {
            //        foreach (var d in docs) options.SwaggerDoc(d, new Info { Version = d });
            //        options.DocInclusionPredicate((docName, description) =>
            //        {
            //            description.TryGetMethodInfo(out MethodInfo mi);
            //            var attr = mi.DeclaringType.GetCustomAttribute<ApiExplorerSettingsAttribute>();
            //            if (attr != null)
            //            {
            //                return attr.GroupName == docName;
            //            }
            //            else
            //            {
            //                return docName == "未分类";
            //            }
            //        });
            //        var ss = options.SwaggerGeneratorOptions;
            //        options.IncludeXmlComments(@"F:\扩展资料\OA_System_2019\OA_System_Server\Sys.OA.Service\Sys.OA.API\bin\Debug\netcoreapp2.2\Sys.OA.API.xml");
            //    });
            //}
            //跨域设置  
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //app.UseMvc();
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NumberOne API V1");
                c.RoutePrefix = string.Empty;
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.DocumentTitle = "Ron.liang Swagger 测试文档";
                    foreach (var item in docs)
                        options.SwaggerEndpoint($"/swagger/{item}/swagger.json", item);
                });
            }
            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
