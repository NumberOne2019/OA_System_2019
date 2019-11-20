using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FytSoa.Common;
using Sys.OA.Service;
using Sys.OA.Service.Implements;
using Sys.OA.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Sys.OA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        private readonly UserInfoService _userInfoService;
        public ValuesController( UserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }
        [HttpGet("list")]
        public JsonResult GetList(int pageIndex, int pageSize)
        {
            SqlSugar.PageModel page = new SqlSugar.PageModel();
            page.PageIndex = pageIndex;
            page.PageSize = pageSize;
            PageParm page2 = new PageParm();
            page2.page = pageIndex;
            page2.limit = pageSize;
            var query = _userInfoService.GetPagesAsync(page2,false);
            //var query2 = _consultService.GetPagesAsync(page2, false).Result.data;
            return Json(query);

        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
