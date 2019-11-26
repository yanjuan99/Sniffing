package com.example.fhook;

public class KeyData {
    private byte[] key;
    private byte[] newData;
    private byte[] oldData;

    public KeyData(byte[] key, byte[] oldData, byte[] newData) {
        this.key = key;
        this.oldData = oldData;
        this.newData = newData;
    }

    public byte[] getKey() {
        return this.key;
    }

    public void setKey(byte[] key) {
        this.key = key;
    }

    public byte[] getOldData() {
        return this.oldData;
    }

    public void setOldData(byte[] oldData) {
        this.oldData = oldData;
    }

    public byte[] getNewData() {
        return this.newData;
    }

    public void setNewData(byte[] newData) {
        this.newData = newData;
    }
}
