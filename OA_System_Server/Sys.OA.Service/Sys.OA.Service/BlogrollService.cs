using System;
using System.Collections.Generic;
using System.Text;
using Gc.CoreSys.Core;
using GcSite.BackSys.Models;
using Sugar.Enties;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;

namespace Gc.CoreSys.Service
{
    public class BlogrollService : DbContext<Blogrolls>
    {
        public override List<Blogrolls> GetList(Expression<Func<Blogrolls, bool>> where = null)
        {
            if (where == null)
                return CurrentDb.GetList();
            else
                return CurrentDb.GetList(where);
        }
        public async Task<List<Blogrolls>> GetListAsnyc(Expression<Func<Blogrolls, bool>> where = null)
        {
            return await Db.Queryable<Blogrolls>().Where(where).ToListAsync();
        }
        public List<GcSite.BackSys.Models.Blogroll> GetBlogroll()
        {
            //Gc.CoreSys.Core.GcSiteDb db = new GcSiteDb();
            //return db.Blogrolls.AsQueryable().ToList();
            using (WorkOfUnit work = new WorkOfUnit())
            {
                return work.CreateRepository<GcSite.BackSys.Models.Blogroll>().GetList().Select(m => new {
                    m.Id,
                    m.Lock,
                    m.BlogrollName,
                    m.Link,
                    m.Sort,
                    m.Sign,
                    m.ReleaseDate
                }) as List<GcSite.BackSys.Models.Blogroll>;
            }
        }
        public List<Blogrolls> GetPageList(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var query = GetList().ToList();
                query.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
                return query.ToList();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

                string sql = string.Empty;
                Db.Aop.OnError = (exp) =>//执行SQL 错误事件
                {
                    sql = "" + exp.Sql + exp.Parametres;//可以拿到参数和错误Sql 
                    string sql2 = sql;
                };
                //DB.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
                //{

                //};
            }
        }
    }
}
