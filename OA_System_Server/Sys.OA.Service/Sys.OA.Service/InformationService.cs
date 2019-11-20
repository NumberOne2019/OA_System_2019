using Gc.CoreSys.Core;
using SqlSugar;
using Sugar.Enties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gc.CoreSys.Service
{
    public class InformationService: DbContext<Information>
    {
        public class Page<T>
        {
            /// <summary>
            /// 当前页索引
            /// </summary>
            public long CurrentPage { get; set; }
            /// <summary>
            /// 总页数
            /// </summary>
            public long TotalPages { get; set; }
            /// <summary>
            /// 总记录数
            /// </summary>
            public long TotalItems { get; set; }
            /// <summary>
            /// 每页的记录数
            /// </summary>
            public long ItemsPerPage { get; set; }
            /// <summary>
            /// 数据集
            /// </summary>
            public List<T> Items { get; set; }
        }
        public override List<Information> GetList(Expression<Func<Information, bool>> whereExpression)
        {
            return Db.Queryable<Information>().ToList();
        }
        public async Task<List<Information>> GetListAsnyc(int pageIndex = 1, int pageSize = 10)
        {
            return await Db.Queryable<Information>().Where(m => m.Id != 0).ToPageListAsync(pageIndex,pageSize);
        }
        public Page<Information> GetPageLists(Expression<Func<Information, bool>> whereExpression, PageModel pageModel, Expression<Func<Information, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            //return CurrentDb.GetPageList(whereExpression, pageModel, orderByExpression, orderByType);

            var page = new Page<Information>();
            var totalItems = GetList(whereExpression).ToList();
            var totalPages = totalItems.Count != 0 ? (totalItems.Count % pageModel.PageSize) == 0 ? (totalItems.Count / pageModel.PageSize) : (totalItems.Count / pageModel.PageSize) + 1 : 0;
            page.ItemsPerPage = pageModel.PageSize;
            page.TotalItems = totalItems.Count;
            page.TotalPages = totalPages;
            page.Items = totalItems.Count == 0 ? null : totalItems.Skip(pageModel.PageSize * (pageModel.PageIndex - 1)).Take(pageModel.PageSize).ToList();
            page.CurrentPage = pageModel.PageIndex + 1;
            return page;
        }
        public List<GcSite.BackSys.Models.Information> GetInformation() {
            Gc.CoreSys.Core.GcSiteDb db = new GcSiteDb();
            return db.Informations.AsQueryable().ToList();
        }
        //public override List<Information> GetPageList(Expression<Func<Information, bool>> whereExpression,int pageIndex = 1, int pageSize = 10)
        //{
        //    try
        //    {
        //        var query = GetList(whereExpression).ToList();
        //        query.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        //        return query.ToList();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    finally
        //    {

        //        string sql = string.Empty;
        //        Db.Aop.OnError = (exp) =>//执行SQL 错误事件
        //        {
        //            sql = "" + exp.Sql + exp.Parametres;//可以拿到参数和错误Sql 
        //            string sql2 = sql;
        //        };
        //        //DB.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
        //        //{

        //        //};
        //    }
        //}
    }
}
