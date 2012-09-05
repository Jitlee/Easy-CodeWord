using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiboSDK.Sina
{
    public class T : SWeiboRequest
    {
        private OAuth _oAuth;
        public T(OAuth oAuth)
        {
            _oAuth = oAuth;
        }

        public void Update(string msg)
        {
            var parameters = new List<Parameter>();
            var url = string.Concat(Api.BASE_URL, Api.UPDATE_API);
            parameters.Add(new Parameter("access_token", _oAuth.AccessToken));
            parameters.Add(new Parameter("status", msg));
            base.SyncRequest(url, "POST", parameters);
        }
    }
}
