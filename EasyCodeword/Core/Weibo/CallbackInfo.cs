using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiboSDK
{
    public delegate void AsyncRequestCallback(int key, string content);

    //回调信息
    class CallbackInfo
    {
        public int key = 0;
        public AsyncRequestCallback callback = null;
    }
}
