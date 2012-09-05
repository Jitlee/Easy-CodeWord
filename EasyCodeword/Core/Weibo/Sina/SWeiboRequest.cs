using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiboSDK.Sina
{
    public class SWeiboRequest
    {
        //同步http请求
        /// <summary>
        /// 同步http请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="key"></param>
        /// <param name="listParam"></param>
        /// <param name="listFile"></param>
        /// <returns></returns>
        public string SyncRequest(string url, string httpMethod, List<Parameter> listParam)
        {
            SyncHttp http = new SyncHttp();
            var queryString = HttpUtil.FormatQueryString(listParam);
            if (httpMethod == "GET")
            {
                return http.HttpGet(url, queryString);
            }
            else
            {
                return http.HttpPost(url, queryString);
            }
        }
    }
}
