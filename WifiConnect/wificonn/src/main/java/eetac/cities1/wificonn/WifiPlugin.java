package eetac.cities1.wificonn;

import android.content.Context;
import android.content.IntentFilter;
import android.net.wifi.WifiConfiguration;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;

import java.util.List;

/**
 * Created by oscar on 07/12/2018.
 */

public class WifiPlugin {
    private static Context context;
    private static WifiBroadcastReceiver wifiBroadcastReceiver;

    public static final int WEP = 1;
    public static final int WPA = 2;
    public static final int NONE = 3;

    public static void Init(Context context)
    {
        WifiPlugin.context = context;
    }

    public static boolean tryConnect(String ssid, String pass, int type, final ConnectionCallback callback){

        boolean ret = true;
        WifiManager wifiManager = (WifiManager)context.getSystemService(Context.WIFI_SERVICE);
        if(wifiManager.isWifiEnabled())
        {
            WifiInfo wifiInfo = wifiManager.getConnectionInfo();
            String SSID = wifiInfo.getSSID();
            if(SSID.equals("\"" + ssid + "\"")) {
                if(wifiBroadcastReceiver==null){
                    IntentFilter intentFilter = new IntentFilter();
                    intentFilter.addAction(WifiManager.NETWORK_STATE_CHANGED_ACTION);
                    WifiPlugin.wifiBroadcastReceiver = new WifiBroadcastReceiver(type,callback);
                    context.registerReceiver(wifiBroadcastReceiver, intentFilter);
                }
                callback.connected();
                return true;
            }
        }
        IntentFilter intentFilter = new IntentFilter();
        intentFilter.addAction(WifiManager.NETWORK_STATE_CHANGED_ACTION);
        if(wifiBroadcastReceiver!=null){
            context.unregisterReceiver(wifiBroadcastReceiver);
        }
        WifiPlugin.wifiBroadcastReceiver = new WifiBroadcastReceiver(type,callback);
        context.registerReceiver(wifiBroadcastReceiver, intentFilter);
        ret = connect(ssid, pass, type);
        return ret;
    }

    public static void stopConnection(){
        WifiManager wifiManager = (WifiManager)context.getSystemService(Context.WIFI_SERVICE);
        wifiManager.disconnect();
        context.unregisterReceiver(wifiBroadcastReceiver);
        wifiBroadcastReceiver = null;
    }


    public static boolean connect(String ssid,String pass,int type){
        WifiConfiguration conf = new WifiConfiguration();
        conf.SSID = "\"" + ssid + "\"";
        switch(type){
            case WEP:
                conf.wepKeys[0] = "\"" + pass + "\"";
                conf.wepTxKeyIndex = 0;
                conf.allowedKeyManagement.set(WifiConfiguration.KeyMgmt.NONE);
                conf.allowedGroupCiphers.set(WifiConfiguration.GroupCipher.WEP40);
                break;

            case WPA:
                conf.preSharedKey = "\""+ pass +"\"";
                break;

            case NONE:
                conf.allowedKeyManagement.set(WifiConfiguration.KeyMgmt.NONE);
                break;
        }

        WifiManager wifiManager = (WifiManager)context.getSystemService(Context.WIFI_SERVICE);
        if(!wifiManager.isWifiEnabled())
            wifiManager.setWifiEnabled(true);
        wifiManager.addNetwork(conf);

        List<WifiConfiguration> list = wifiManager.getConfiguredNetworks();
        for( WifiConfiguration i : list ) {
            if(i.SSID != null && i.SSID.equals("\"" + ssid + "\"")) {
                wifiManager.disconnect();
                wifiManager.enableNetwork(i.networkId, true);
                wifiManager.reconnect();

                break;
            }
        }
        return false;
    }

    public interface ConnectionCallback{
        public void connected();
        public void disconnected();
    }
}
