using Sys.OA.Service.Repository;
using Sugar.Enties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FytSoa.Common;
using FytSoa.Service.DtoModel;

namespace Sys.OA.Service.Interfaces
{
    public interface IUserInfoService : IBaseServer<UserInfo>
    {

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        Task<ApiResult<SysAdminMenuDto>> LoginAsync(SysAdminLogin parm);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<Page<UserInfo>>> GetPagesAsync(PageParm parm);

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> AddAsync(UserInfo parm);

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> DeleteAsync(string parm);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<string>> ModifyAsync(UserInfo parm);
    }
}
