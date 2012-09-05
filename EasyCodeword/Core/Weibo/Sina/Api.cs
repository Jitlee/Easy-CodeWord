using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiboSDK.Sina
{
    public class Api
    {
        public readonly static string AUTHORIZE_URL = "https://api.weibo.com/oauth2/authorize";

        public readonly static string BASE_URL = "https://api.weibo.com/2/";

        #region 用户

        // 根据用户ID获取用户信息
        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        public const string SHOW_API = "users/show.json";

        // 批量获取用户的粉丝数、关注数、微博数
        /// <summary>
        /// 批量获取用户的粉丝数、关注数、微博数
        /// </summary>
        public const string COUNTS_API = "users/counts.json";

        #endregion

        #region 微博

        // 发布一条新微博
        /// <summary>
        /// 发布一条新微博
        /// </summary>
        public const string UPDATE_API = "statuses/update.json";

        #endregion
    }
}
