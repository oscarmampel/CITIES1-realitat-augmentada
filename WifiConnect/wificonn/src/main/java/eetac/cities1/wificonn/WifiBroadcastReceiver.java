package eetac.cities1.wificonn;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.NetworkInfo;
import android.net.wifi.WifiManager;
import android.util.Log;

/**
 * Created by oscar on 08/12/2018.
 */

public class WifiBroadcastReceiver extends BroadcastReceiver {
    WifiFSM wifiFSM;
    WifiBroadcastReceiver(int type, WifiPlugin.ConnectionCallback connectionCallback){
        this.wifiFSM = new WifiFSM(type,connectionCallback);
    }

    @Override
    public void onReceive(Context context, Intent intent) {
        final String action = intent.getAction();
        if (action.equals(WifiManager.NETWORK_STATE_CHANGED_ACTION)) {
            NetworkInfo info = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
            NetworkInfo.DetailedState state = info.getDetailedState();
            Log.d("WIFI",state.name());
            switch (state.name()){
                case "AUTHENTICATING":
                    wifiFSM.updateState(State.AUTHENTICATING);
                    break;
                case "OBTAINING_IPADDR":
                    wifiFSM.updateState(State.OBTAINING_IPADDR);
                    break;
                case "CONNECTED":
                    wifiFSM.updateState(State.CONNECTED);
                    break;
                case "DISCONNECTED":
                    wifiFSM.updateState(State.DISCONNECTED);
                    break;
                case "CONNECTING":
                    wifiFSM.updateState(State.CONNECTING);
                    break;
            }
        }
    }

    enum State{
        IDLE,
        CONNECTING,
        AUTHENTICATING,
        OBTAINING_IPADDR,
        CONNECTED,
        DISCONNECTED
    }

    private class WifiFSM{
        int type;
        State state;
        WifiPlugin.ConnectionCallback callback;
        WifiFSM(int type, WifiPlugin.ConnectionCallback callback){
            this.type = type;
            this.state = State.IDLE;
            this.callback = callback;
        }

        void updateState(State state){
            if(type == WifiPlugin.NONE){
                if(state == State.CONNECTING){
                    this.state = State.CONNECTING;
                }
                if(this.state == State.CONNECTING && state == State.OBTAINING_IPADDR){
                    this.state = State.OBTAINING_IPADDR;
                }
                if(this.state == State.OBTAINING_IPADDR && state == State.CONNECTED){
                    this.state = State.CONNECTED;
                    callback.connected();
                }
            }

            if(type == WifiPlugin.WEP||type == WifiPlugin.WPA){
                if(state == State.AUTHENTICATING){
                    this.state = State.AUTHENTICATING;
                }
                if(this.state == State.AUTHENTICATING && state == State.OBTAINING_IPADDR){
                    this.state = State.OBTAINING_IPADDR;
                }
                if(this.state == State.OBTAINING_IPADDR && state == State.CONNECTED){
                    this.state = State.CONNECTED;
                    callback.connected();
                }
            }
            if(this.state == State.CONNECTED &&state == State.DISCONNECTED){
                callback.disconnected();
                this.state = State.DISCONNECTED;
                //context.unregisterReceiver(wifiBroadcastReceiver);
            }
        }
    }

}