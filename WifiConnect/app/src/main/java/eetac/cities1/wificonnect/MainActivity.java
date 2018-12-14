package eetac.cities1.wificonnect;

import android.os.Debug;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import eetac.cities1.wificonn.WifiPlugin;


public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        WifiPlugin.Init(this.getApplicationContext());
        /*WifiPlugin.tryConnect("Arduino-Uno-WiFi-11B7B7", null, WifiPlugin.NONE, new WifiPlugin.ConnectionCallback() {
            @Override
            public void connected() {
                Log.d("WIFI","connected");
            }

            @Override
            public void disconnected() {
                Log.d("WIFI","disconneted");
            }
        });*/
        WifiPlugin.tryConnect("ADAMO-D3B4", "6U5WX5LCCDTFMQ", WifiPlugin.WPA, new WifiPlugin.ConnectionCallback() {
            @Override
            public void connected() {
                Toast toast = Toast.makeText(MainActivity.this, "Connected", Toast.LENGTH_LONG);
                Log.d("WIFI","connected");
            }

            @Override
            public void disconnected() {
                Toast toast = Toast.makeText(MainActivity.this, "no Connected", Toast.LENGTH_LONG);
                Log.d("WIFI","disconneted");
                WifiPlugin.stopConnection();
            }
        });
        Toast toast = Toast.makeText(MainActivity.this, "Connected", Toast.LENGTH_LONG);
        //Log.d("WIFI","connected");
        //wifiPlugin.connect("ADAMO-D3B4","6U5WX5LCCDTFMQ", WifiPlugin.WPA);


    }
}
