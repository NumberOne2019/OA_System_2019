using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gc.CoreSys.Web.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            List<string> list = new List<string>();
            list.Add("0");
            list.Add("1");
            list.Add("2");
            list.Add("3");
        }
    }
}
