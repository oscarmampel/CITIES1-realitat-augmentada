using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WifiConn
{
    public class WifiConn
    {

        public static readonly int WEP = 1;
        public static readonly int WPA = 2;
        public static readonly int NONE = 3;

        WifiConn()
        {
        }

        public static void Init()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (var androidPlugin = new AndroidJavaClass("eetac.cities1.wificonn.WifiPlugin"))
                        {
                            androidPlugin.CallStatic("Init", currentActivity);
                        }
                    }
                }
            }
        }

        public static bool TryConnect(string ssid, string pass, int type,IConnectionCallback connectionCallback)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (var androidPlugin = new AndroidJavaClass("eetac.cities1.wificonn.WifiPlugin"))
                        {
                            ConnectionCallbackProxy conn = new ConnectionCallbackProxy(connectionCallback);
                            return androidPlugin.CallStatic<bool>("tryConnect", ssid,pass,type,conn);
                        }
                    }
                }
            }
            return false;
        }

        public static void StopConnection()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (var androidPlugin = new AndroidJavaClass("eetac.cities1.wificonn.WifiPlugin"))
                        {
                            androidPlugin.CallStatic("stopConnection");
                        }
                    }
                }
            }
        }




        private class ConnectionCallbackProxy : AndroidJavaProxy
        {
            IConnectionCallback connectionCallback;
            public ConnectionCallbackProxy(IConnectionCallback connectionCallback) : base("eetac.cities1.wificonn.WifiPlugin$ConnectionCallback") {
                this.connectionCallback = connectionCallback;
            }
            void connected()
            {
                connectionCallback.Connect();
            }

            void disconnected()
            {
                connectionCallback.Disconnect();
            }
        }
    }
}