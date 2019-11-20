﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using GcSite.BackSys.Models;
using Microsoft.EntityFrameworkCore;

namespace Gc.CoreSys.Core
{
    public class WorkOfUnit : IDisposable
    {
        private GcSiteDb _db;
        private Dictionary<Type, object> _respositories;
        public WorkOfUnit()
        {
            this._db = new GcSiteDb();
            this._respositories = new Dictionary<Type, object>();
        }

        public IRepository<TEntity> CreateRepository<TEntity>()
            where TEntity : EntityBase
        {
            IRepository<TEntity> res = null;
            if (_respositories.ContainsKey(typeof(TEntity)))
            {
                res = _respositories[typeof(TEntity)] as IRepository<TEntity>;
            }
            else
            {
                res = new GenericRepository<TEntity>(this._db);
                _respositories.Add(typeof(TEntity), res);
            }
            return res;
        }

        //#region Query
        //public List<T> Query<T>(string sql, params SqlParameter[] paras)
        //{
        //    return this._db.Database.SqlQuery<T>(sql, paras).ToList();
        //}
        //#endregion

        //#region QueryByProc
        //public List<T> QueryByProc<T>(string procName, params SqlParameter[] paras)
        //{
        //    string sql = GetProc(procName, paras);
        //    return this._db.Database.SqlQuery<T>(sql, paras).ToList();
        //}
        //#endregion

        //#region GetProc
        //private static string GetProc(string procName, SqlParameter[] paras)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    sql.AppendFormat("EXEC [dbo].[{0}] ", procName);
        //    foreach (var item in paras)
        //    {
        //        sql.AppendFormat("{0} {1},", item.ParameterName,
        //            item.Direction == ParameterDirection.Output ? "OUTPUT" : "");
        //    }
        //    if (paras.Length > 0)
        //    {
        //        sql.Remove(sql.Length - 1, 1);
        //    }
        //    return sql.ToString();
        //}
        //#endregion

        //#region ExecuteNonQueryByProc
        //public int ExecuteNonQueryByProc(string procName, params SqlParameter[] paras)
        //{
        //    string sql = GetProc(procName, paras);
        //    return _db.Database.ExecuteSqlCommand(sql, paras);
        //}
        //#endregion

        //#region ExecuteNonQuery
        //public int ExecuteNonQuery(string sql, params SqlParameter[] paras)
        //{
        //    return _db.Database.ExecuteSqlCommand(sql, paras);
        //}
        //#endregion

        #region SaveChanges
        public int Save()
        {
            return _db.SaveChanges();
        }
        #endregion

        #region SaveChangesAsync
        public Task<int> SaveAsync()
        {
            return _db.SaveChangesAsync();
        }
        #endregion

        #region RollBackChanges
        public void RollBackChanges()
        {
            var items = _db.ChangeTracker.Entries().ToList();
            items.ForEach(o => o.State = EntityState.Unchanged);
        }
        #endregion

        #region GC
        private bool _disposeState = false;
        protected virtual void Dispose(bool dispose)
        {
            if (_disposeState)
            {
                if (dispose)
                {
                    this._db.Dispose();
                }
                _disposeState = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
