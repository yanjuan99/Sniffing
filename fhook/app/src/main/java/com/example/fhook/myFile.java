package com.example.fhook;

import android.os.Environment;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStreamReader;

public  class myFile {
    static  String arry[];
    public static boolean WriteIP (String ip,Integer port)
    {
        try {
            String Data = ip + ":"+  port.toString();

            String sdCardDir = Environment.getExternalStorageDirectory().getAbsolutePath()+"/tmpip.txt";
            File file=new  File(sdCardDir );
            if (!file.exists())
            {
                file.createNewFile();
            }
            FileOutputStream out =new FileOutputStream(file,false);
            out.write(Data.getBytes("UTF-8"));
            out.close();
            return  true;
        }
        catch (Exception e)
        {
            e.printStackTrace();
            return false;
        }
    }
    public static boolean ReadIP ( )
    {
        try {
            String sdCardDir = Environment.getExternalStorageDirectory().getAbsolutePath()+"/tmpip.txt";
            File file=new  File(sdCardDir );
            if (!file.exists())
            {
                return  false;
            }
            FileInputStream in =new FileInputStream(file);
            BufferedReader  bufferedReader = new BufferedReader(new InputStreamReader(in));
            String data = bufferedReader.readLine();
            bufferedReader.close();
            in.close();

            arry = data.split(":");
            return  true;
        }
        catch (Exception e)
        {
            e.printStackTrace();
            return false;
        }
    }
}
