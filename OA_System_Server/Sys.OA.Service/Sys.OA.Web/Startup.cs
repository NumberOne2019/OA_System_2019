using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FytSoa.Common;
using Gc.CoreSys.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Gc.CoreSys.Web
{
    public class BaseConfigModel
    {
        /// <summary>
        /// 
        /// </summary>
        public static IConfiguration Configuration { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public static string ContentRootPath { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public static string WebRootPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="contentRootPath"></param>
        /// <param name="webRootPath"></param>
        public static void SetBaseConfig(IConfiguration config, string contentRootPath, string webRootPath)
        {
            Configuration = config;
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            this.Configuration = builder.Build();
            BaseConfigModel.SetBaseConfig(Configuration, env.ContentRootPath, env.WebRootPath);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //自定注册
            AddAssembly(services, "Gc.CoreSys.Service");

            services.AddDbContext<GcSiteDb>(options =>
        options.UseSqlServer(ConfigurationManager.ConnectionStrings["connStr"].ConnectionString));
            //DbContext.DB_ConnectionString = "uid=sa;Password=Gc262314;database=ddDb;Server=192.168.1.200;Connect Timeout=30;";   //为数据库连接字符串赋值
            services.AddTransient<Gc.CoreSys.Service.BlogrollService>();
            services.AddTransient<Gc.CoreSys.Service.InformationService>();

            #region 授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy("App", policy => policy.RequireRole("App").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("AdminOrApp", policy => policy.RequireRole("Admin,App").Build());
            });
            #endregion


            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/web/index", "/");
            });


            #region CORS
            services.AddCors(c =>
            {
                c.AddPolicy("Any", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });

                c.AddPolicy("Limit", policy =>
                {
                    policy
                    .WithOrigins("localhost:54849")
                    .WithMethods("get", "post", "put", "delete")
                    //.WithHeaders("Authorization");
                    .AllowAnyHeader();
                });
            });
            #endregion

            #region 性能 压缩
            services.AddResponseCompression();
            #endregion
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});


            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            //添加NLog  
            loggerFactory.AddNLog();
            //读取Nlog配置文件 
            env.ConfigureNLog("nlog.config");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //自定义异常处理
            app.UseMiddleware<ExceptionFilter>();
            //认证
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
        /// <summary>  
        /// 自动注册服务——获取程序集中的实现类对应的多个接口
        /// </summary>
        /// <param name="services">服务集合</param>  
        /// <param name="assemblyName">程序集名称</param>
        public void AddAssembly(IServiceCollection services, string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType).ToList();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    if (interfaceType.Length == 1)
                    {
                        services.AddTransient(interfaceType[0], item);
                    }
                    if (interfaceType.Length > 1)
                    {
                        services.AddTransient(interfaceType[1], item);
                    }
                }
            }
        }
    }

    [Serializable]
    public class MyException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        private int _code;
        /// <summary>
        /// 
        /// </summary>
        public MyException()
        {
            _code = 400;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public MyException(string message, int code = 400)
            : base(message)
        {
            _code = code;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="code"></param>
        public MyException(string message, Exception inner, int code = 400)
            : base(message, inner)
        {
            _code = code;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCode()
        {
            return _code;
        }
    }
    public class ExceptionFilter
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ExceptionFilter(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            bool isCatched = false;
            try
            {
                await _next(context);
            }
            catch (Exception ex) //发生异常
            {
                //自定义业务异常
                if (ex is MyException)
                {
                    context.Response.StatusCode = ((MyException)ex).GetCode();
                }
                //未知异常
                else
                {
                    context.Response.StatusCode = 500;
                    //LogHelper.SetLog(LogLevel.Error, ex);
                }
                //记录异常日志
                Logger.Default.ProcessError(context.Response.StatusCode, ex.Message);
                await HandleExceptionAsync(context, context.Response.StatusCode, ex.Message);
                isCatched = true;
            }
            finally
            {
                if (!isCatched && context.Response.StatusCode != 200)//未捕捉过并且状态码不为200
                {
                    string msg = "";
                    switch (context.Response.StatusCode)
                    {
                        case 401:
                            msg = "未授权";
                            break;
                        case 404:
                            msg = "未找到服务";
                            break;
                        case 502:
                            msg = "请求错误";
                            break;
                        default:
                            msg = "未知错误";
                            break;
                    }
                    Logger.Default.ProcessError(context.Response.StatusCode, msg);
                    await HandleExceptionAsync(context, context.Response.StatusCode, msg);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string msg)
        {
            var data = new { statusCode, Success = false, message = msg };
            context.Response.ContentType = "application/json;charset=utf-8";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(data));
        }
    }
}
