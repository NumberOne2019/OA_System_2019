using GcSite.BackSys.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Gc.CoreSys.Core
{
    public partial class GcSiteDb : Microsoft.EntityFrameworkCore.DbContext
    {
        public GcSiteDb()
        {
        }

        public GcSiteDb(DbContextOptions<GcSiteDb> options)
            : base(options)
        {
        }
        #region 管理员模块
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        #endregion

        #region 资讯模块
        public DbSet<LgClass> LgClasses { get; set; }
        public DbSet<SmClass> SmClasses { get; set; }
        public DbSet<Information> Informations { get; set; }
        public DbSet<Recommend> Recommends { get; set; }
        public DbSet<RmationImg> RmationImgs { get; set; }
        #endregion

        #region 新闻模块
        //public DbSet<NewsType> NewsTypes { get; set; }
        //public DbSet<NewsInfo> NewsInfoes { get; set; }
        #endregion

        #region 留言反馈模块
        //public DbSet<MessageType> MessageTypes { get; set; }
        public DbSet<MessageInfo> MessageInfoes { get; set; }
        #endregion

        #region 产品模块
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductImg> ProductImgs { get; set; }
        public DbSet<ProductInfo> ProductInfos { get; set; }

        #region 视频模块

        public DbSet<VideoLg> VideoLg { get; set; }
        public DbSet<VideoSm> VideoSm { get; set; }
        public DbSet<VideoInfo> VideoInfoes { get; set; }
        #endregion

        #endregion

        #region 广告模块
        public DbSet<AdverType> AdverTypes { get; set; }
        public DbSet<AdeverInfo> AdeverInfoes { get; set; }
        #endregion

        #region 友情链接模块
        public DbSet<Blogroll> Blogrolls { get; set; }
        #endregion

        #region 招聘模块
        public DbSet<Recruited> Recruiteds { get; set; }
        #endregion

        #region 栏目模块
        public DbSet<Programa> Programas { get; set; }
        public DbSet<SecondPrograma> SecondProgramas { get; set; }
        #endregion

        #region 法律申明
        public DbSet<Statement> Statement { get; set; }
        #endregion

        #region 日志
        public DbSet<Log> Logs { get; set; }
        #endregion

        #region 营销系统模块
        public DbSet<GcSite.BackSys.Models.Service> Services { get; set; }
        public DbSet<AnchorText> AnchorText { get; set; }
        public DbSet<VisitInfo> VisitInfoes { get; set; }
        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("uid=sa;Password=Gc262314;database=ddDb;Server=192.168.1.200;Connect Timeout=30;");//ConfigurationManager.ConnectionStrings["connStr"].ConnectionString
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
