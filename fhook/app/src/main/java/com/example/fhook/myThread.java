package com.example.fhook;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import de.robv.android.xposed.XposedBridge;

public class myThread extends Thread
{
    private String str;
    public myThread(String s )
    {
        this.str = s;
    }
    public void run()
    {
        try {
            myFile.ReadIP();
            Socket socket =  new Socket(myFile.arry[0],Integer.parseInt( myFile.arry[1]));
            BufferedReader in= new BufferedReader(new InputStreamReader(socket.getInputStream()));
            PrintWriter out=new PrintWriter(socket.getOutputStream());

            out.print(str);
            out.flush();

            in.read();

            in.close();
            out.close();
            socket.close();
        } catch (Exception e) {
            XposedBridge.log("run:" + e.toString());
        }
    }
}