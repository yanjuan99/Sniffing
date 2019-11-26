package com.example.fhook;


public class mySocket {
    public static int sendmsg(String str)
    {
        Thread thread = new myThread(str);
        thread.start();
        return 1;
    }
}


