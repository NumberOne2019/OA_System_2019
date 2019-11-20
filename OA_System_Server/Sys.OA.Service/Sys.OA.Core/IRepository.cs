using GcSite.BackSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gc.CoreSys.Core
{
    public interface IRepository<TEntity>
        where TEntity : EntityBase
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        void Insert(TEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="excludeFields"></param>
        void Update(TEntity entity, params string[] excludeFields);
        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="id"></param>
        void Delete(object id);
        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        TEntity GetEntityById(object id);

        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> where = null);

        IEnumerable<TEntity> GetPageList<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true);

        Task<IEnumerable<TEntity>> GetPageAsync<TKey>(Expression<Func<TEntity, bool>> whereLambda, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true);

        TEntity GetFirst(Expression<Func<TEntity, bool>> where = null);

        int GetCount(Expression<Func<TEntity, bool>> where = null);
    }
}
