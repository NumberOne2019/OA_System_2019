using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FytSoa.Common;
using FytSoa.Service.DtoModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Sugar.Enties;
using Sys.OA.Extensions;
using Sys.OA.Service.Implements;
using Sys.OA.Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sys.OA.API.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserInfoService _userInfoService;
        public AccountController(UserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpGet("login")]
        [AllowAnonymous]
        public ApiResult<string> Login([FromBody]SysAdminLogin parm)
        {
            var apiRes = new ApiResult<string>() { statusCode = (int)ApiEnum.HttpRequestError };
            var token = "";
            try
            {
                ////获得公钥私钥，解密
                //var rsaKey = MemoryCacheService.Default.GetCache<List<string>>("LOGINKEY");
                //if (rsaKey == null)
                //{
                //    apiRes.message = "登录失败，请刷新浏览器再次登录";
                //    return apiRes;
                //}
                ////Ras解密密码
                //var ras = new RSACrypt(rsaKey[0], rsaKey[1]);
                //parm.password = ras.Decrypt(parm.password);

                //获得用户登录限制次数
                var configLoginCount = Convert.ToInt32(ConfigExtensions.Configuration[KeyHelper.LOGINCOUNT]);
                //获得登录次数和过期时间
                var loginConfig = MemoryCacheService.Default.GetCache<SysAdminLoginConfig>(KeyHelper.LOGINCOUNT) ?? new SysAdminLoginConfig();
                if (loginConfig.Count != 0 && loginConfig.DelayMinute != null)
                {
                    //说明存在过期时间，需要判断
                    if (DateTime.Now <= loginConfig.DelayMinute)
                    {
                        apiRes.message = "您的登录以超过设定次数，请稍后再次登录~";
                        return apiRes;
                    }
                    else
                    {
                        //已经过了登录的预设时间，重置登录配置参数
                        loginConfig.Count = 0;
                        loginConfig.DelayMinute = null;
                    }
                }
                //查询登录结果
                var dbres = _userInfoService.LoginAsync(parm).Result;
                if (dbres.statusCode != 200)
                {
                    //增加登录次数
                    loginConfig.Count += 1;
                    //登录的次数大于配置的次数，则提示过期时间
                    if (loginConfig.Count == configLoginCount)
                    {
                        var configDelayMinute = Convert.ToInt32(ConfigExtensions.Configuration[KeyHelper.LOGINDELAYMINUTE]);
                        //记录过期时间
                        loginConfig.DelayMinute = DateTime.Now.AddMinutes(configDelayMinute);
                        apiRes.message = "登录次数超过" + configLoginCount + "次，请" + configDelayMinute + "分钟后再次登录";
                        return apiRes;
                    }
                    //记录登录次数，保存到session
                    MemoryCacheService.Default.SetCache(KeyHelper.LOGINCOUNT, loginConfig);
                    //提示用户错误和登录次数信息
                    apiRes.message = dbres.message + "　　您还剩余" + (configLoginCount - loginConfig.Count) + "登录次数";
                    return apiRes;
                }

                var user = dbres.data.admin;
                var identity = new ClaimsPrincipal(
                 new ClaimsIdentity(new[]
                     {
                              new Claim(ClaimTypes.Sid,user.UserId.ToString()),
                              new Claim(ClaimTypes.Role,user.RoleId.ToString()),
                              new Claim(ClaimTypes.Thumbprint,user.UserImage),
                              new Claim(ClaimTypes.Name,user.LoginName),
                              new Claim(ClaimTypes.WindowsAccountName,user.LoginName),
                              new Claim(ClaimTypes.UserData,user.UpLoginDate.ToString())
                     }, CookieAuthenticationDefaults.AuthenticationScheme)
                );
                //如果保存用户类型是Session，则默认设置cookie退出浏览器 清空
                if (ConfigExtensions.Configuration[KeyHelper.LOGINSAVEUSER] == "Session")
                {
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
                    {
                        AllowRefresh = false
                    });
                }
                else
                {
                    //根据配置保存浏览器用户信息，小时单位
                    var hours = int.Parse(ConfigExtensions.Configuration[KeyHelper.LOGINCOOKIEEXPIRES]);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddHours(hours),
                        IsPersistent = true,
                        AllowRefresh = false
                    });
                }

                //把权限存到缓存里
                var menuSaveType = ConfigExtensions.Configuration[KeyHelper.LOGINAUTHORIZE];
                if (menuSaveType == "Redis")
                {
                    RedisHelper.Set(KeyHelper.ADMINMENU + "_" + dbres.data.admin.UserId, dbres.data.menu);
                }
                else
                {
                    MemoryCacheService.Default.SetCache(KeyHelper.ADMINMENU + "_" + dbres.data.admin.UserId, dbres.data.menu, 600);
                }
                token = JwtHelper.IssueJWT(new TokenModel()
                {
                    Uid = user.UserId.ToString(),
                    UserName = user.LoginName,
                    Role = "Admin",
                    TokenType = "Web"
                });
                MemoryCacheService.Default.RemoveCache("LOGINKEY");
                MemoryCacheService.Default.RemoveCache(KeyHelper.LOGINCOUNT);

                #region 保存日志
                //var agent = HttpContext.Request.Headers["User-Agent"];
                //var log = new SysLog()
                //{
                //    Guid = Guid.NewGuid().ToString(),
                //    Logged = DateTime.Now,
                //    Logger = LogEnum.LOGIN.GetEnumText(),
                //    Level = "Info",
                //    Message = "登录",
                //    Callsite = "/fytadmin/login",
                //    IP = Utils.GetIp(),
                //    User = parm.loginname,
                //    Browser = agent.ToString()
                //};
                //_logService.AddAsync(log);
                #endregion
            }
            catch (Exception ex)
            {
                apiRes.message = ex.Message;
                apiRes.statusCode = (int)ApiEnum.Error;

                //#region 保存日志
                //var agent = HttpContext.Request.Headers["User-Agent"];
                //var log = new SysLog()
                //{
                //    Guid = Guid.NewGuid().ToString(),
                //    Logged = DateTime.Now,
                //    Logger = LogEnum.LOGIN.GetEnumText(),
                //    Level = "Error",
                //    Message = "登录失败！" + ex.Message,
                //    Exception = ex.Message,
                //    Callsite = "/fytadmin/login",
                //    IP = Utils.GetIp(),
                //    User = parm.loginname,
                //    Browser = agent.ToString()
                //};
                //_logService.AddAsync(log);
                //#endregion
            }
            apiRes.statusCode = (int)ApiEnum.Status;
            apiRes.data = token;
            return apiRes;
        }

        /// <summary>
        /// 管理员退出
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout"), Log("Admin：LogOut", LogType = LogEnum.LOGOUT)]
        [AllowAnonymous]
        public ApiResult<string> LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new ApiResult<string>() { data = "/account/login/" };
        }
    }
}
