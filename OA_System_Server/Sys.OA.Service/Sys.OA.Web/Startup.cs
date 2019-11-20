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
            //�Զ�ע��
            AddAssembly(services, "Gc.CoreSys.Service");

            services.AddDbContext<GcSiteDb>(options =>
        options.UseSqlServer(ConfigurationManager.ConnectionStrings["connStr"].ConnectionString));
            //DbContext.DB_ConnectionString = "uid=sa;Password=Gc262314;database=ddDb;Server=192.168.1.200;Connect Timeout=30;";   //Ϊ���ݿ������ַ�����ֵ
            services.AddTransient<Gc.CoreSys.Service.BlogrollService>();
            services.AddTransient<Gc.CoreSys.Service.InformationService>();

            #region ��Ȩ
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

            #region ���� ѹ��
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

            //���NLog  
            loggerFactory.AddNLog();
            //��ȡNlog�����ļ� 
            env.ConfigureNLog("nlog.config");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //�Զ����쳣����
            app.UseMiddleware<ExceptionFilter>();
            //��֤
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
        /// <summary>  
        /// �Զ�ע����񡪡���ȡ�����е�ʵ�����Ӧ�Ķ���ӿ�
        /// </summary>
        /// <param name="services">���񼯺�</param>  
        /// <param name="assemblyName">��������</param>
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
            catch (Exception ex) //�����쳣
            {
                //�Զ���ҵ���쳣
                if (ex is MyException)
                {
                    context.Response.StatusCode = ((MyException)ex).GetCode();
                }
                //δ֪�쳣
                else
                {
                    context.Response.StatusCode = 500;
                    //LogHelper.SetLog(LogLevel.Error, ex);
                }
                //��¼�쳣��־
                Logger.Default.ProcessError(context.Response.StatusCode, ex.Message);
                await HandleExceptionAsync(context, context.Response.StatusCode, ex.Message);
                isCatched = true;
            }
            finally
            {
                if (!isCatched && context.Response.StatusCode != 200)//δ��׽������״̬�벻Ϊ200
                {
                    string msg = "";
                    switch (context.Response.StatusCode)
                    {
                        case 401:
                            msg = "δ��Ȩ";
                            break;
                        case 404:
                            msg = "δ�ҵ�����";
                            break;
                        case 502:
                            msg = "�������";
                            break;
                        default:
                            msg = "δ֪����";
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
