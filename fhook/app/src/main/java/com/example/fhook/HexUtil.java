package com.example.fhook;

import android.annotation.SuppressLint;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.util.regex.Matcher;
import java.util.regex.Pattern;


public class HexUtil {
    private static String hexString = "0123456789ABCDEF";

    private static byte charToByte(char c) {
        return (byte) hexString.indexOf(c);
    }

    public static byte[] hexStrToBytes(String hexStr) {
        hexStr = hexStr.replaceAll(" ", "").replaceAll("\n", "").toUpperCase();
        int length = hexStr.length() / 2;
        char[] hexChars = hexStr.toCharArray();
        byte[] d = new byte[length];
        for (int i = 0; i < length; i++) {
            int pos = i * 2;
            d[i] = (byte) ((charToByte(hexChars[pos]) << 4) | charToByte(hexChars[pos + 1]));
        }
        return d;
    }

    public static String bytesToHexStr(byte[] b) {
        String ret = "";
        for (byte b2 : b) {
            String hex = Integer.toHexString(b2 & 0xff);
            if (hex.length() == 1) {
                hex = new StringBuilder(String.valueOf('0')).append(hex).toString();
            }
            ret = new StringBuilder(String.valueOf(new StringBuilder(String.valueOf(ret)).append(hex.toUpperCase()).toString())).append(" ").toString();
        }
        return ret;
    }

    public static String formatHexStr(String str) {
        String resultStr = "";
        return bytesToHexStr(hexStrToBytes(str.replaceAll(" ", "").replaceAll("\n", "")));
    }

    public static String formatHexStrSingle(String str) {
        String resultStr = "";
        return str.replaceAll(" ", "").replaceAll("\n", "");
    }


    public static String replaceBytes(String str) {
        String strNew = str;
        System.out.println(str);
        Matcher matcher = Pattern.compile("\\[([\\d|,|\\-|\\s]+)\\]").matcher(str);
        while (matcher.find()) {
            String temp = str.substring(matcher.start(), matcher.end());
            String[] arr = temp.replace("[", "").replace("]", "").split(",");
            byte[] bytes = new byte[arr.length];
            for (int i = 0; i < arr.length; i++) {
                bytes[i] = Byte.valueOf(arr[i]).byteValue();
            }
            strNew = strNew.replace(temp, "\"" + HexUtil.bytesToHexStr(bytes) + "\"");
        }
        return strNew;
    }

}