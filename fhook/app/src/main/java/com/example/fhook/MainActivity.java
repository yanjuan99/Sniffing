package com.example.fhook;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;
import de.robv.android.xposed.XposedBridge;

import static android.widget.Toast.LENGTH_LONG;

public class MainActivity extends AppCompatActivity {
    private Button btn_c;
    private Button btn_a;
    private EditText ip;
    private EditText port;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        if (!isModuleActive()){
            Toast.makeText(this, "模块未启动", LENGTH_LONG).show();

        }
        else {
            Toast.makeText(this, "模块已启动", LENGTH_LONG).show();
        }

        if (ContextCompat.checkSelfPermission(MainActivity.this, Manifest.permission.WRITE_EXTERNAL_STORAGE)
                != PackageManager.PERMISSION_GRANTED) {
            //申请WRITE_EXTERNAL_STORAGE权限
            ActivityCompat.requestPermissions(MainActivity.this,
                    new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                    1);
        }
        myFile.ReadIP();
        ip=(EditText) findViewById(R.id.edit_ip);
        port=(EditText)findViewById(R.id.edit_port);

        btn_c=(Button)findViewById(R.id.btn_Connect);
        btn_c.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                try {
                    if (1!=mySocket.sendmsg("123456"))
                    {
                        Toast.makeText(MainActivity.this, "err" , LENGTH_LONG).show();
                    }
                }catch ( Exception e)
                {
                    XposedBridge.log("onClick:" + e.getMessage());
                }
            }
        });

        btn_a=(Button)findViewById(R.id.btn_a);
        btn_a.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                myFile.WriteIP(ip.getText().toString(),Integer.parseInt(port.getText().toString()));
            }
        });
    }
    private boolean isModuleActive(){
        return false;
    }
}
