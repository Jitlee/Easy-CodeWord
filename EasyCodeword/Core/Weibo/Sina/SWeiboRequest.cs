using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiboSDK.Sina
{
    public class SWeiboRequest
    {
        private Dictionary<AsyncHttp, CallbackInfo> asyncRquestMap = new Dictionary<AsyncHttp, CallbackInfo>();
        private int key = 0;

        //同步http请求
        public string SyncRequest(string url, string httpMethod, List<Parameter> listParam, List<Parameter> listFile)
        {

            string queryString = HttpUtil.FormatQueryString(listParam);

            SyncHttp http = new SyncHttp();
            if (httpMethod == "GET")
            {
                return http.HttpGet(url, queryString);
            }
            else if ((listFile == null) || (listFile.Count == 0))
            {
                return http.HttpPost(url, queryString);
            }
            else
            {
                return http.HttpPostWithFile(url, queryString, listFile);
            }
        }

        //异步http请求
        public bool AsyncRequest(string url, string httpMethod, List<Parameter> listParam, List<Parameter> listFile,
            AsyncRequestCallback callback, out int callbkey)
        {
            string queryString = HttpUtil.FormatQueryString(listParam);

            AsyncHttp http = new AsyncHttp();

            callbkey = GetKey();
            CallbackInfo callbackInfo = new CallbackInfo();
            callbackInfo.key = callbkey;
            callbackInfo.callback = callback;

            asyncRquestMap.Add(http, callbackInfo);

            bool bResult = false;

            if (httpMethod == "GET")
            {
                bResult = http.HttpGet(url, queryString, new AsyncHttpCallback(HttpCallback));
            }
            else if ((listFile == null) || (listFile.Count == 0))
            {
                bResult = http.HttpPost(url, queryString, new AsyncHttpCallback(HttpCallback));
            }
            else
            {
                bResult = http.HttpPostWithFile(url, queryString, listFile, new AsyncHttpCallback(HttpCallback));
            }

            if (!bResult)
            {
                asyncRquestMap.Remove(http);
            }
            return bResult;
        }

        //回调
        protected void HttpCallback(AsyncHttp http, string content)
        {
            CallbackInfo info;
            if (!asyncRquestMap.TryGetValue(http, out info))
            {
                return;
            }

            if ((info != null) && (info.callback != null))
            {
                info.callback(info.key, content);
            }
            asyncRquestMap.Remove(http);
        }

        private int GetKey()
        {
            return ++key;
        }
    }
}
