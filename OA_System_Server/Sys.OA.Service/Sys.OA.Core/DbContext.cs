using FytSoa.Common;
using SqlSugar;
using Sugar.Enties;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sys.OA.Core
{
    public class DbContext<T> where T : class, new()
    {
        public static string DB_ConnectionString { get; set; }

        //public static SqlSugarClient Db
        //{
        //    get => new SqlSugarClient(new ConnectionConfig()
        //    {
        //        ConnectionString = DB_ConnectionString,
        //        DbType = DbType.SqlServer,
        //        IsAutoCloseConnection = true,
        //        InitKeyType = InitKeyType.SystemTable,
        //        IsShardSameThread = true
        //    }
        // );
        //}
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigExtensions.Configuration["DbConnection:MySqlConnectionString"],
                DbType = DbType.SqlServer,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了

            });

        }
        //注意：不能写成静态的
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public SimpleClient<T> CurrentDb { get { return new SimpleClient<T>(Db); } }//用来操作当前表的数据

        public SimpleClient<UserInfo> UserInfoDb { get { return new SimpleClient<UserInfo>(Db); } }//用来处理UserInfo表的常用操作
        public SimpleClient<Personalfile> PersonalfileDb { get { return new SimpleClient<Personalfile>(Db); } }//用来处理Personalfile表的常用操作
        public SimpleClient<RoleInfo> RoleInfoDb { get { return new SimpleClient<RoleInfo>(Db); } }//用来处理RoleInfo表的常用操作
        public SimpleClient<RoleRight> RoleRightDb { get { return new SimpleClient<RoleRight>(Db); } }//用来处理RoleRight表的常用操作
        public SimpleClient<NavInfo> NavInfoDb { get { return new SimpleClient<NavInfo>(Db); } }//用来处理NavInfo表的常用操作
        public SimpleClient<BranchInfo> BranchInfoDb { get { return new SimpleClient<BranchInfo>(Db); } }//用来处理BranchInfo表的常用操作
        public SimpleClient<DepartInfo> DepartInfoDb { get { return new SimpleClient<DepartInfo>(Db); } }//用来处理DepartInfo表的常用操作
        public SimpleClient<FileInfo> FileInfoDb { get { return new SimpleClient<FileInfo>(Db); } }//用来处理FileInfo表的常用操作
        public SimpleClient<ManualSign> ManualSignDb { get { return new SimpleClient<ManualSign>(Db); } }//用来处理ManualSign表的常用操作
        public SimpleClient<TableTag> TableTagDb { get { return new SimpleClient<TableTag>(Db); } }//用来处理TableTag表的常用操作
        public SimpleClient<MeetingRoom> MeetingRoomDb { get { return new SimpleClient<MeetingRoom>(Db); } }//用来处理MeetingRoom表的常用操作
        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetList()
        {
            return CurrentDb.GetList();
        }

        /// <summary>
        /// 根据表达式查询
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return CurrentDb.GetList(whereExpression);
        }


        /// <summary>
        /// 根据表达式查询分页
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel pageModel)
        {
            return CurrentDb.GetPageList(whereExpression, pageModel);
        }

        /// <summary>
        /// 根据表达式查询分页并排序
        /// </summary>
        /// <param name="whereExpression">it</param>
        /// <param name="pageModel"></param>
        /// <param name="orderByExpression">it=>it.id或者it=>new{it.id,it.name}</param>
        /// <param name="orderByType">OrderByType.Desc</param>
        /// <returns></returns>
        public virtual List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel pageModel, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return CurrentDb.GetPageList(whereExpression, pageModel, orderByExpression, orderByType);
        }


        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetById(dynamic id)
        {
            return CurrentDb.GetById(id);
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(dynamic id)
        {
            return CurrentDb.Delete(id);
        }


        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(T data)
        {
            return CurrentDb.Delete(data);
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(dynamic[] ids)
        {
            return CurrentDb.AsDeleteable().In(ids).ExecuteCommand() > 0;
        }

        /// <summary>
        /// 根据表达式删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            return CurrentDb.Delete(whereExpression);
        }


        /// <summary>
        /// 根据实体更新，实体需要有主键
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Update(T obj)
        {
            return CurrentDb.Update(obj);
        }

        /// <summary>
        ///批量更新
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Update(List<T> objs)
        {
            return CurrentDb.UpdateRange(objs);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Insert(T obj)
        {
            return CurrentDb.Insert(obj);
        }


        /// <summary>
        /// 批量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Insert(List<T> objs)
        {
            return CurrentDb.InsertRange(objs);
        }
    }
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class DbContext
    {
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigExtensions.Configuration["DbConnection:MySqlConnectionString"],
                DbType = DbType.SqlServer,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了
            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                string s = sql;
                //Console.WriteLine(sql + "\r\n" +
                //    Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                //Console.WriteLine();
            };
        }
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作

        public SimpleClient<UserInfo> UserInfoDb { get { return new SimpleClient<UserInfo>(Db); } }//用来处理UserInfo表的常用操作
        public SimpleClient<Personalfile> PersonalfileDb { get { return new SimpleClient<Personalfile>(Db); } }//用来处理Personalfile表的常用操作
        public SimpleClient<RoleInfo> RoleInfoDb { get { return new SimpleClient<RoleInfo>(Db); } }//用来处理RoleInfo表的常用操作
        public SimpleClient<RoleRight> RoleRightDb { get { return new SimpleClient<RoleRight>(Db); } }//用来处理RoleRight表的常用操作
        public SimpleClient<NavInfo> NavInfoDb { get { return new SimpleClient<NavInfo>(Db); } }//用来处理NavInfo表的常用操作
        public SimpleClient<BranchInfo> BranchInfoDb { get { return new SimpleClient<BranchInfo>(Db); } }//用来处理BranchInfo表的常用操作
        public SimpleClient<DepartInfo> DepartInfoDb { get { return new SimpleClient<DepartInfo>(Db); } }//用来处理DepartInfo表的常用操作
        public SimpleClient<FileInfo> FileInfoDb { get { return new SimpleClient<FileInfo>(Db); } }//用来处理FileInfo表的常用操作
        public SimpleClient<ManualSign> ManualSignDb { get { return new SimpleClient<ManualSign>(Db); } }//用来处理ManualSign表的常用操作
        public SimpleClient<TableTag> TableTagDb { get { return new SimpleClient<TableTag>(Db); } }//用来处理TableTag表的常用操作
        public SimpleClient<MeetingRoom> MeetingRoomDb { get { return new SimpleClient<MeetingRoom>(Db); } }//用来处理MeetingRoom表的常用操作
    }
}
