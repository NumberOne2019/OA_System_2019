using Sys.OA.Service.Interfaces;
using Sys.OA.Service.Repository;
using Sugar.Enties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FytSoa.Common;
using FytSoa.Service.DtoModel;
using Sys.OA.Service.Extensions;

namespace Sys.OA.Service.Implements
{
    public class UserInfoService:BaseService<UserInfo>, IUserInfoService
    {
        #region  用户登录和授权菜单查询
        /// <summary>
        /// 用户登录实现
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<SysAdminMenuDto>> LoginAsync(SysAdminLogin parm)
        {
            var res = new ApiResult<SysAdminMenuDto>() { statusCode = (int)ApiEnum.Error };
            try
            {
                var adminModel = new SysAdminMenuDto();
                parm.password = DES3Encrypt.EncryptString(parm.password);
                var model = await Db.Queryable<UserInfo>()
                        .Where(m => m.LoginName == parm.loginname).FirstAsync();
                if (model == null)
                {
                    res.message = "账号错误";
                    return res;
                }
                if (!model.PassWord.Equals(parm.password))
                {
                    res.message = "密码错误~";
                    return res;
                }
                if (model.IsDelete == 0)
                {
                    res.message = "登录账号被冻结，请联系管理员~";
                    return res;
                }
                //adminModel.menu = GetMenuByAdmin(model.UserId);
                //if (adminModel == null)
                //{
                //    res.message = "当前账号没有授权功能模块，无法登录~";
                //    return res;
                //}
                ////修改登录时间
                //model.LoginDate = DateTime.Now;
                //model.UpLoginDate = model.LoginDate;
                //SysAdminDb.Update(model);



                res.statusCode = (int)ApiEnum.Status;
                adminModel.admin = model;
                res.data = adminModel;
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }
        /// <summary>
        /// 根据登录账号，返回菜单信息
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        List<SysMenuDto> GetMenuByAdmin(string admin)
        {
            List<SysMenuDto> res = null;
            try
            {
                //根据用户查询角色列表， 一个用户对应多个角色
                ////var roleList = SysPermissionsDb.GetList(m => m.AdminGuid == admin && m.Types == 2).Select(m => m.RoleGuid).ToList();
                //////根据角色查询菜单，并查询到菜单涉及的功能
                ////var query = Db.Queryable<SysMenu, SysPermissions>((sm, sp) => new object[]{
                ////    JoinType.Left,sm.Guid==sp.MenuGuid
                ////})
                ////.Where((sm, sp) => roleList.Contains(sp.RoleGuid) && sp.Types == 1 && sm.Status)
                ////.OrderBy((sm, sp) => sm.Sort)
                ////.Select((sm, sp) => new SysMenuDto()
                ////{
                ////    guid = sm.Guid,
                ////    parentGuid = sm.ParentGuid,
                ////    parentName = sm.ParentName,
                ////    name = sm.Name,
                ////    nameCode = sm.NameCode,
                ////    parentGuidList = sm.ParentGuidList,
                ////    layer = sm.Layer,
                ////    urls = sm.Urls,
                ////    icon = sm.Icon,
                ////    sort = sm.Sort,
                ////    btnJson = sp.BtnFunJson
                ////})
                ////.Mapper((it, cache) => {
                ////    var codeList = cache.Get(list =>
                ////    {
                ////        return Db.Queryable<SysCode>().Where(m => m.ParentGuid == "a88fa4d3-3658-4449-8f4a-7f438964d716")
                ////            .Select(m => new SysCodeDto()
                ////            {
                ////                guid = m.Guid,
                ////                name = m.Name,
                ////                codeType = m.CodeType
                ////            })
                ////            .ToList();
                ////    });
                ////    if (!string.IsNullOrEmpty(it.btnJson))
                ////    {
                ////        it.btnFun = codeList.Where(m => it.btnJson.Contains(m.guid)).ToList();
                ////    }
                //});
                //res = query.ToList();
            }
            catch
            {
                res = null;
            }
            return res;
        }
        #endregion

        /// <summary>
        /// 添加部门信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> AddAsync(UserInfo parm)
        {
            var res = new ApiResult<string>
            {
                statusCode = (int)ApiEnum.ParameterError
            };
            try
            {
                //判断用吗是否存在
                var isExisteName = await Db.Queryable<UserInfo>().AnyAsync(m => m.LoginName == parm.LoginName);
                if (isExisteName)
                {
                    res.message = "用户名已存在，请更换~";
                    return res;
                }
                parm.PassWord = DES3Encrypt.EncryptString(parm.PassWord);
                if (string.IsNullOrEmpty(parm.UserImage))
                {
                    parm.UserImage = "/themes/img/avatar.jpg";
                }
                //parm.Guid = Guid.NewGuid().ToString();
                //parm.AddDate = DateTime.Now;
                //if (!string.IsNullOrEmpty(parm.DepartmentGuid))
                //{
                //    // 说明有父级  根据父级，查询对应的模型
                //    var model = SysOrganizeDb.GetById(parm.DepartmentGuid);
                //    parm.DepartmentGuidList = model.ParentGuidList;
                //}
                //res.statusCode = (int)ApiEnum.Status;
                //SysAdminDb.Insert(parm);
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> DeleteAsync(string parm)
        {
            int isok = 0;
            //var list = Utils.StrToListString(parm);
            //var isok = await Db.Deleteable<UserInfo>().Where(m => list(m.UserId)).ExecuteCommandAsync();
            ////删除授权
            //if (isok > 1)
            //{
            //    await Db.Deleteable<SysPermissions>().Where(m => list.Contains(m.MenuGuid) && m.Types == 2).ExecuteCommandAsync();
            //}
            var res = new ApiResult<string>
            {
                statusCode = isok > 0 ? 200 : 500,
                data = isok > 0 ? "1" : "0",
                message = isok > 0 ? "删除成功~" : "删除失败~"
            };
            return res;
        }


        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<Page<UserInfo>>> GetPagesAsync(PageParm parm)
        {
            var res = new ApiResult<Page<UserInfo>>();
            try
            {
                res.data = await Db.Queryable<UserInfo>()
                        .WhereIF(!string.IsNullOrEmpty(parm.key), m => m.UserName.Contains(parm.key))
                        .OrderBy(m => m.IsDelete).ToPageAsync(parm.page, parm.limit);
            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                res.statusCode = (int)ApiEnum.Error;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> ModifyAsync(UserInfo parm)
        {
            var res = new ApiResult<string>
            {
                statusCode = (int)ApiEnum.Error
            };
            try
            {
                //修改，判断用户是否和其它的重复
                var isExisteName = await Db.Queryable<UserInfo>().AnyAsync(m => m.LoginName == parm.LoginName && m.UserId != parm.UserId);
                if (isExisteName)
                {
                    res.message = "用户名已存在，请更换~";
                    res.statusCode = (int)ApiEnum.ParameterError;
                    return await Task.Run(() => res);
                }

                parm.PassWord = DES3Encrypt.EncryptString(parm.PassWord);
                //if (!string.IsNullOrEmpty(parm.DepartmentGuid))
                //{
                //    // 说明有父级  根据父级，查询对应的模型
                //    var model = SysOrganizeDb.GetById(parm.DepartmentGuid);
                //    parm.DepartmentGuidList = model.ParentGuidList;
                //}
                //var dbres = await Db.Updateable<SysAdmin>().UpdateColumns(m => new SysAdmin()
                //{
                //    LoginName = parm.LoginName,
                //    LoginPwd = parm.LoginPwd,
                //    DepartmentName = parm.DepartmentName,
                //    DepartmentGuid = parm.DepartmentGuid,
                //    DepartmentGuidList = parm.DepartmentGuidList,
                //    TrueName = parm.TrueName,
                //    Number = parm.Number,
                //    Sex = parm.Sex,
                //    Mobile = parm.Mobile,
                //    Email = parm.Email,
                //    Status = parm.Status
                //}).Where(m => m.Guid == parm.Guid).ExecuteCommandAsync();
                //if (dbres > 0)
                //{
                //    res.statusCode = (int)ApiEnum.Status;
                //    res.message = "更新成功！";
                //}
                //else
                //{
                //    res.message = "更新失败！";
                //}

            }
            catch (Exception ex)
            {
                res.message = ApiEnum.Error.GetEnumText() + ex.Message;
                Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        Task<ApiResult<Page<UserInfo>>> IUserInfoService.GetPagesAsync(PageParm parm)
        {
            throw new NotImplementedException();
        }
        
    }
}
