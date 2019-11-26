package com.example.fhook;

import android.widget.Toast;

import de.robv.android.xposed.IXposedHookLoadPackage;
import de.robv.android.xposed.XC_MethodHook;
import de.robv.android.xposed.XC_MethodReplacement;
import de.robv.android.xposed.XposedBridge;
import de.robv.android.xposed.XposedHelpers;
import de.robv.android.xposed.callbacks.XC_LoadPackage;
import com.google.gson.Gson;
import org.json.JSONArray;
import org.json.JSONObject;
import com.example.fhook.HexUtil;
import com.example.fhook.KeyData;
import com.example.fhook.mySocket;
import  com.example.fhook.MainActivity;
import static android.widget.Toast.LENGTH_LONG;


public class HookTest implements IXposedHookLoadPackage {
    public void handleLoadPackage(XC_LoadPackage.LoadPackageParam lpparam) throws Throwable {

        if (lpparam.packageName.equals("com.example.fhook"))
        {
            XposedHelpers.findAndHookMethod(
                    "com.example.fhook.MainActivity",
                    lpparam.classLoader,
                    "isModuleActive",
                    XC_MethodReplacement.returnConstant(true)
            );
        }
        if (lpparam.packageName.equals("com.tencent.mobileqq"))
        {
            XposedHelpers.findAndHookMethod(
                    "oicq.wlogin_sdk.tools.EcdhCrypt",
                    lpparam.classLoader,
                    "GenECDHKeyEx",
                    String.class, String.class, String.class,
                    new XC_MethodHook() {
                        protected void beforeHookedMethod(MethodHookParam param) throws Throwable {
                            mySocket.sendmsg("GenECDHKeyEx : "+ new Gson().toJson(param.args[0]));
                            super.beforeHookedMethod(param);
                        }
                    });

            XposedHelpers.findAndHookMethod(
                    "oicq.wlogin_sdk.tools.EcdhCrypt",
                    lpparam.classLoader,
                    "set_c_pub_key",
                    byte[].class,
                    new XC_MethodHook() {
                        protected void beforeHookedMethod(MethodHookParam param) throws Throwable {
                            mySocket.sendmsg("set_c_pub_key : "+new Gson().toJson(HexUtil.bytesToHexStr((byte[])param.args[0])));
                            super.beforeHookedMethod(param);

                        }
                    });

            XposedHelpers.findAndHookMethod(
                    "oicq.wlogin_sdk.tools.EcdhCrypt",
                    lpparam.classLoader,
                    "set_c_pri_key",
                    byte[].class,
                    new XC_MethodHook() {
                        protected void beforeHookedMethod(MethodHookParam param) throws Throwable {
                            mySocket.sendmsg("set_c_pri_key : "+ new Gson().toJson(HexUtil.bytesToHexStr((byte[])param.args[0])));
                            super.beforeHookedMethod(param);
                        }
                    });

            XposedHelpers.findAndHookMethod(
                    "oicq.wlogin_sdk.tools.EcdhCrypt",
                    lpparam.classLoader,
                    "set_g_share_key",
                    byte[].class,
                    new XC_MethodHook() {
                        protected void beforeHookedMethod(MethodHookParam param) throws Throwable {
                            mySocket.sendmsg("set_g_share_key : "+ new Gson().toJson(HexUtil.bytesToHexStr((byte[])param.args[0])));
                            super.beforeHookedMethod(param);
                        }
                    });

            XposedHelpers.findAndHookMethod(
                    "mqq.app.Packet",
                    lpparam.classLoader,
                    "toMsg",
                    new XC_MethodHook() {
                        protected void afterHookedMethod(MethodHookParam param) throws Throwable {
                        super.afterHookedMethod(param);
                        if (param.getResult()!= null) {
                            mySocket.sendmsg("toMsg : "+ HexUtil.replaceBytes(new Gson().toJson(param.getResult())));
                        }
                    }
                });

            XposedHelpers.findAndHookMethod(
                    "oicq.wlogin_sdk.tools.cryptor",
                    lpparam.classLoader,
                    "decrypt",
                    byte[].class, Integer.TYPE, Integer.TYPE, byte[].class,
                    new XC_MethodHook() {
                        protected void afterHookedMethod(MethodHookParam param) throws Throwable {
                            super.afterHookedMethod(param);
                            if (param.args[0] != null && param.args[3] != null) {
                                byte[] bArr = (byte[]) param.args[0];
                                int i = ((Integer) param.args[1]).intValue();
                                int i2 = ((Integer) param.args[2]).intValue();
                                byte[] bArr2 = (byte[]) param.args[3];
                                byte[] obj = new byte[i2];
                                System.arraycopy(bArr, i, obj, 0, i2);
                                byte[] obj2 = new byte[bArr2.length];
                                System.arraycopy(bArr2, 0, obj2, 0, bArr2.length);
                                //mySocket.sendmsg("decryptKey"+ HexUtil.bytesToHexStr(obj2));
                                mySocket.sendmsg("decrypt"+ HexUtil.replaceBytes( new Gson().toJson(new KeyData(obj2, obj, (byte[]) param.getResult()))));
                            }
                        }
                    }
            );

            XposedHelpers.findAndHookMethod(
                    "oicq.wlogin_sdk.tools.cryptor",
                    lpparam.classLoader,
                    "encrypt",
                    byte[].class, Integer.TYPE, Integer.TYPE, byte[].class,
                    new XC_MethodHook() {
                protected void afterHookedMethod(MethodHookParam param) throws Throwable {
                    super.afterHookedMethod(param);
                    if (param.args[0] != null && param.args[3] != null) {
                        byte[] bArr = (byte[]) param.args[0];
                        int i = ((Integer) param.args[1]).intValue();
                        int i2 = ((Integer) param.args[2]).intValue();
                        byte[] bArr2 = (byte[]) param.args[3];
                        byte[] obj = new byte[i2];
                        System.arraycopy(bArr, i, obj, 0, i2);
                        byte[] obj2 = new byte[bArr2.length];
                        System.arraycopy(bArr2, 0, obj2, 0, bArr2.length);
                        //mySocket.sendmsg("encryptKey"+ HexUtil.bytesToHexStr(obj2));
                        mySocket.sendmsg("encrypt"+ HexUtil.replaceBytes(new Gson().toJson(new KeyData(obj2, obj, (byte[]) param.getResult()))));
                    }
                }
            });

            XposedHelpers.findAndHookMethod(
                    "com.tencent.mobileqq.msf.core.auth.a",
                    lpparam.classLoader,
                    "n",
                    byte[].class, new XC_MethodHook() {
                        protected void beforeHookedMethod(MethodHookParam param) throws Throwable {
                        String jsonStr = new Gson().toJson(param.thisObject);
                        JSONArray arr = new JSONObject(jsonStr).getJSONArray("j");
                        byte[] sessionKey = new byte[arr.length()];
                        for (int i = 0; i < arr.length(); i++) {
                            sessionKey[i] = (byte) arr.getInt(i);
                        }
                            mySocket.sendmsg("sessionKey"+ HexUtil.bytesToHexStr(sessionKey));
                            mySocket.sendmsg("account"+ jsonStr);
                        super.beforeHookedMethod(param);
                }
            });

            XposedHelpers.findAndHookConstructor("oicq.wlogin_sdk.request.Ticket",
                    lpparam.classLoader,
                    Integer.TYPE, byte[].class, byte[].class, Long.TYPE, byte[].class, byte[].class,
                    new XC_MethodHook() {
                    protected void afterHookedMethod(MethodHookParam param) throws Throwable {
                        super.afterHookedMethod(param);
                        mySocket.sendmsg("Ticket"+new Gson().toJson(param.thisObject));
                    }
                });

        }
    }
}
