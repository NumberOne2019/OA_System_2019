using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FytSoa.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sys.OA.Extensions.Middleware
{
    public class MyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch _stopwatch;
        /// <summary>
        /// AES加密
        /// </summary>
        private AESCrypt _aesCrypt = new AESCrypt();
        //加密解密key
        private readonly string securitykey = "0123456789abcdef";
        /// <summary>
        /// 构造http请求中间件
        /// </summary>
        /// <remarks>
        /// 需要实现一个构造函数,参数为 RequestDelegate
        /// </remarks>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public MyMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<MyMiddleware>();
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                List<string> verifList = new List<string>() { "pageindex", "pagesize" };
                int key_count = context.Request.Form.Keys.Count;
                if (key_count != 0)
                {
                    foreach (var item in context.Request.Form.Keys)
                    {
                        if (!verifList.Contains(item))
                        {
                            await context.Response.WriteAsync("非法数据请求，请重新尝试！");
                        }
                    }
                }
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await _next.Invoke(context);
            }
        }
        /// <summary>
        /// 1：将Post方法中Body中的数据进行AES解密
        /// 2：将返回数据进行AES加密
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        //    public async Task InvokeAsync(HttpContext context)
        //    {
        //        context.Request.EnableBuffering();
        //        _stopwatch = new Stopwatch();
        //        _stopwatch.Start();
        //        _logger.LogInformation($"Handling request: " + context.Request.Path);
        //        var api = new ApiRequestInputViewModel
        //        {
        //            HttpType = context.Request.Method,
        //            Query = context.Request.QueryString.Value,
        //            RequestUrl = context.Request.Path,
        //            RequestName = "",
        //            RequestIP = context.Request.Host.Value
        //        };

        //        var request = context.Request.Body;
        //        var response = context.Response.Body;

        //        try
        //        {
        //            using (var newRequest = new MemoryStream())
        //            {
        //                //替换request流
        //                context.Request.Body = newRequest;

        //                using (var newResponse = new MemoryStream())
        //                {
        //                    //替换response流
        //                    context.Response.Body = newResponse;

        //                    using (var reader = new StreamReader(request))
        //                    {
        //                        //读取原始请求流的内容
        //                        api.Body = await reader.ReadToEndAsync();
        //                        if (string.IsNullOrEmpty(api.Body))
        //                            await _next.Invoke(context);
        //                        //示例加密字符串，使用 AES-ECB-PKCS7 方式加密，密钥为：0123456789abcdef
        //                        // 加密参数：{"value":"哈哈哈"}
        //                        // 加密后数据： oedwSKGyfLX8ADtx2Z8k1Q7+pIoAkdqllaOngP4TvQ4=
        //                        api.Body = _aesCrypt.Decrypt(api.Body, securitykey);
        //                    }
        //                    using (var writer = new StreamWriter(newRequest))
        //                    {
        //                        await writer.WriteAsync(api.Body);
        //                        await writer.FlushAsync();
        //                        newRequest.Position = 0;
        //                        context.Request.Body = newRequest;
        //                        await _next.Invoke(context);
        //                    }

        //                    using (var reader = new StreamReader(newResponse))
        //                    {
        //                        newResponse.Position = 0;
        //                        api.ResponseBody = await reader.ReadToEndAsync();
        //                        if (!string.IsNullOrWhiteSpace(api.ResponseBody))
        //                        {
        //                            api.ResponseBody = _aesCrypt.Encrypt(api.ResponseBody, securitykey);
        //                        }
        //                    }
        //                    using (var writer = new StreamWriter(response))
        //                    {
        //                        await writer.WriteAsync(api.ResponseBody);
        //                        await writer.FlushAsync();
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($" http中间件发生错误: " + ex.ToString());
        //        }
        //        finally
        //        {
        //            context.Request.Body = request;
        //            context.Response.Body = response;
        //        }

        //        // 响应完成时存入缓存
        //        context.Response.OnCompleted(() =>
        //        {
        //            _stopwatch.Stop();
        //            api.ElapsedTime = _stopwatch.ElapsedMilliseconds;

        //            _logger.LogDebug($"RequestLog:{DateTime.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(0, 10000)}-{api.ElapsedTime}ms", $"{JsonConvert.SerializeObject(api)}");
        //            return Task.CompletedTask;
        //        });

        //        _logger.LogInformation($"Finished handling request.{_stopwatch.ElapsedMilliseconds}ms");
        //    //}
    }
    public class ApiRequestInputViewModel
    {
        public string HttpType { get; set; }

        public string Query { get; set; }

        public string RequestUrl { get; set; }

        public string RequestName { get; set; }

        public string RequestIP { get; set; }

        public string ResponseBody { get; set; }

        public string Body { get; set; }

        public long ElapsedTime { get; set; }
    }
}
