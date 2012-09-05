using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiboSDK.Sina
{
    public class User : SWeiboRequest
    {
        private OAuth _oAuth;
        public User(OAuth oAuth)
        {
            _oAuth = oAuth;
        }

        public string Info()
        {
            var parameters = new List<Parameter>();
            var url = string.Concat(Api.BASE_URL, Api.SHOW_API);
            parameters.Add(new Parameter("access_token", _oAuth.AccessToken));
            parameters.Add(new Parameter("uid", _oAuth.Uid));
            return base.SyncRequest(url, "GET", parameters);
        }

        public string Counts()
        {
            var parameters = new List<Parameter>();
            var url = string.Concat(Api.BASE_URL, Api.COUNTS_API);
            parameters.Add(new Parameter("access_token", _oAuth.AccessToken));
            parameters.Add(new Parameter("uids", _oAuth.Uid));
            return base.SyncRequest(url, "GET", parameters);
        }
    }
}
