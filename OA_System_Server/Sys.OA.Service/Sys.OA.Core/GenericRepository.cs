using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using GcSite.BackSys.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Gc.CoreSys.Core
{
    public class GenericRepository<TEntity> : IRepository<TEntity>
         where TEntity : EntityBase
    {
        internal Microsoft.EntityFrameworkCore.DbContext _db;
        internal DbSet<TEntity> _dbSet;
        public GenericRepository(Microsoft.EntityFrameworkCore.DbContext context)
        {
            this._db = context;
            this._dbSet = this._db.Set<TEntity>();
        }
        public void Delete(object id)
        {
            TEntity entity = GetEntityById(id);
            Delete(entity);
        }

        public void Delete(TEntity entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public int GetCount(Expression<Func<TEntity, bool>> where = null)
        {
            if (where == null)
                return _dbSet.Count();
            else
                return _dbSet.Count(where);
        }

        public TEntity GetEntityById(object id)
        {
            return this._dbSet.Find(id);//根据主键查找实体
        }

        public TEntity GetFirst(Expression<Func<TEntity, bool>> where = null)
        {
            return _dbSet.Where(where).First();
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> where = null)
        {
            IEnumerable<TEntity> query = null;
            if (where == null)
            {
                query = _dbSet;
            }
            else
            {
                query = _dbSet.Where(where);
            }
            return query;
        }

        public IEnumerable<TEntity> GetPageList<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> orderBy, int pageIndex = 1, int pageSize = 10, bool isOrder = true)
        {
            IQueryable<TEntity> data = isOrder ?
                _dbSet.OrderBy(orderBy) :
                _dbSet.OrderByDescending(orderBy);
            return data.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }
        #region 分页查找
        /// <summary>
        /// 分页查询异步
        /// </summary>
        /// <param name="whereLambda">查询添加（可有，可无）</param>
        /// <param name="ordering">排序条件（一定要有）</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="isOrder">排序正反</param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetPageAsync<TKey>(Expression<Func<TEntity, bool>> whereLambda, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            IQueryable<TEntity> data = isOrder ?
                _dbSet.OrderBy(orderBy) :
                _dbSet.OrderByDescending(orderBy);

            if (whereLambda != null)
            {
                data = isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
            }
            return await data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        ///// <summary>
        ///// 分页查询异步
        ///// </summary>
        ///// <param name="whereLambda">查询添加（可有，可无）</param>
        ///// <param name="ordering">排序条件（一定要有，多个用逗号隔开，倒序开头用-号）</param>
        ///// <param name="pageIndex">当前页码</param>
        ///// <param name="pageSize">每页大小</param>
        ///// <returns></returns>
        //public async Task<IEnumerable<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> whereLambda, string ordering, int pageIndex, int pageSize, bool isNoTracking = true)
        //{
        //    // 分页 一定注意： Skip 之前一定要 OrderBy
        //    if (string.IsNullOrEmpty(ordering))
        //    {
        //        ordering = nameof(TEntity) + "Id";//默认以Id排序
        //    }
        //    var data = _dbSet.o(ordering);
        //    if (whereLambda != null)
        //    {
        //        data = isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
        //    }
        //    //查看生成的sql，找到大数据下分页巨慢原因为order by 耗时
        //    //var sql = data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToSql();
        //    //File.WriteAllText(@"D:\sql.txt",sql);
        //    IEnumerable<TEntity> pageData = new IEnumerable<TEntity>
        //    {
        //        Totals = await data.CountAsync(),
        //        Rows = await data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
        //    };
        //    return pageData;
        //}

        ///// <summary>
        ///// 分页查询
        ///// </summary>
        ///// <param name="whereLambda">查询添加（可有，可无）</param>
        ///// <param name="ordering">排序条件（一定要有，多个用逗号隔开，倒序开头用-号）</param>
        ///// <param name="pageIndex">当前页码</param>
        ///// <param name="pageSize">每页大小</param>
        ///// <returns></returns>
        //public IEnumerable<TEntity> GetPage(Expression<Func<TEntity, bool>> whereLambda, string ordering, int pageIndex, int pageSize, bool isNoTracking = true)
        //{
        //    // 分页 一定注意： Skip 之前一定要 OrderBy
        //    if (string.IsNullOrEmpty(ordering))
        //    {
        //        ordering = nameof(TEntity) + "Id";//默认以Id排序
        //    }
        //    var data = _dbSet.OrderBy(ordering);
        //    if (whereLambda != null)
        //    {
        //        data = isNoTracking ? data.Where(whereLambda).AsNoTracking() : data.Where(whereLambda);
        //    }
        //    IEnumerable<TEntity> pageData = new IEnumerable<TEntity>
        //    {
        //        Totals = data.Count(),
        //        Rows = data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
        //    };
        //    return pageData;
        //}
        #endregion

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(TEntity entity, params string[] excludeFields)
        {
            EntityEntry entry = this._db.Entry(entity);
            entry.State = EntityState.Modified;
            foreach (var item in excludeFields)
            {
                entry.Property(item).IsModified = false;
            }
        }
    }
}
