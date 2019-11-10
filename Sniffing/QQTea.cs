using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class QQTea
{

    public static string Bytetostr(byte[] In)
    {
        string result = "";
        for (int i = 0; i < In.Length; i++)
        {
            result += In[i].ToString("X2")+" ";
            if ((i + 1) % 16 == 0)
            {
                result += "\r";
            }
        }
        return result;
    }

    public static byte[] Strtobyte(string str)
    {
        List<string> substr = new List<string>();
        str = Regex.Replace(str, @"\s", "");
        int len = str.Length;
        byte[] result = new byte[len / 2];

        if (len != 0 && len % 2 == 0)
        {
            int index = 0;
            do
            {
                substr.Add(str.Substring(index, 2));
                index += 2;
            } while (len != index);
            str = "";
            int i = 0;
            foreach (var item in substr)
            {
                result[i] = Convert.ToByte(item, 16);
                i++;
            }

        }
        return result;
    }

    private static void code(byte[] In, int inOffset, int inPos, byte[] Out, int outOffset, int outPos, byte[] key)
    {
        if (outPos > 0)
        {
            for (int i = 0; i < 8; i++)
            {
                In[(outOffset + outPos) + i] = (byte)(In[(inOffset + inPos) + i] ^ Out[((outOffset + outPos) + i) - 8]);
            }
        }
        uint[] numArray = FormatKey(key);
        uint v = ConvertByteArrayToUInt(In, outOffset + outPos);
        uint num3 = ConvertByteArrayToUInt(In, (outOffset + outPos) + 4);
        uint num4 = 0;
        uint num5 = 0x9e3779b9;
        uint num6 = 0x10;
        while (num6-- > 0)
        {
            num4 += num5;
            v += (((num3 << 4) + numArray[0]) ^ (num3 + num4)) ^ ((num3 >> 5) + numArray[1]);
            num3 += (((v << 4) + numArray[2]) ^ (v + num4)) ^ ((v >> 5) + numArray[3]);
        }
        Array.Copy(ConvertUIntToByteArray(v), 0, Out, outOffset + outPos, 4);
        Array.Copy(ConvertUIntToByteArray(num3), 0, Out, (outOffset + outPos) + 4, 4);
        if (inPos > 0)
        {
            for (int j = 0; j < 8; j++)
            {
                Out[(outOffset + outPos) + j] = (byte)(Out[(outOffset + outPos) + j] ^ In[((inOffset + inPos) + j) - 8]);
            }
        }
    }

    private static uint ConvertByteArrayToUInt(byte[] v, int offset)
    {
        if ((offset + 4) > v.Length)
        {
            return 0;
        }
        uint num = (uint)(v[offset] << 0x18);
        num |= (uint)(v[offset + 1] << 0x10);
        num |= (uint)(v[offset + 2] << 8);
        return (num | v[offset + 3]);
    }

    private static byte[] ConvertUIntToByteArray(uint v)
    {
        return new byte[] { ((byte)((v >> 0x18) & 0xff)), ((byte)((v >> 0x10) & 0xff)), ((byte)((v >> 8) & 0xff)), ((byte)(v & 0xff)) };
    }

    private static void decode(byte[] In, int inOffset, int inPos, byte[] Out, int outOffset, int outPos, byte[] key)
    {
        if (outPos > 0)
        {
            for (int i = 0; i < 8; i++)
            {
                Out[(outOffset + outPos) + i] = (byte)(In[(inOffset + inPos) + i] ^ Out[((outOffset + outPos) + i) - 8]);
            }
        }
        else
        {
            Array.Copy(In, inOffset, Out, outOffset, 8);
        }
        uint[] numArray = FormatKey(key);
        uint v = ConvertByteArrayToUInt(Out, outOffset + outPos);
        uint num3 = ConvertByteArrayToUInt(Out, (outOffset + outPos) + 4);
        uint num4 = 0xe3779b90;
        uint num5 = 0x9e3779b9;
        uint num6 = 0x10;
        while (num6-- > 0)
        {
            num3 -= (((v << 4) + numArray[2]) ^ (v + num4)) ^ ((v >> 5) + numArray[3]);
            v -= (((num3 << 4) + numArray[0]) ^ (num3 + num4)) ^ ((num3 >> 5) + numArray[1]);
            num4 -= num5;
        }
        Array.Copy(ConvertUIntToByteArray(v), 0, Out, outOffset + outPos, 4);
        Array.Copy(ConvertUIntToByteArray(num3), 0, Out, (outOffset + outPos) + 4, 4);
    }

    public static byte[] Decrypt(byte[] In, int offset, int len, byte[] key)
    {
        if (((len % 8) != 0) || (len < 0x10))
        {
            return null;
        }
        byte[] @out = new byte[len];
        for (int i = 0; i < len; i += 8)
        {
            decode(In, offset, i, @out, 0, i, key);
        }
        for (int j = 8; j < len; j++)
        {
            @out[j] = (byte)(@out[j] ^ In[(offset + j) - 8]);
        }
        int num3 = @out[0] & 7;
        len = (len - num3) - 10;
        byte[] destinationArray = new byte[len];
        Array.Copy(@out, num3 + 3, destinationArray, 0, len);
        return destinationArray;
    }

    public static byte[] Encrypt(byte[] In, int offset, int len, byte[] key)
    {
        Random random = new Random();
        int num = (len + 10) % 8;
        if (num != 0)
        {
            num = 8 - num;
        }
        byte[] destinationArray = new byte[(len + num) + 10];
        destinationArray[0] = (byte)((random.Next() & 0xf8) | num);
        for (int i = 1; i < (num + 3); i++)
        {
            destinationArray[i] = (byte)(random.Next() & 0xff);
        }
        Array.Copy(In, 0, destinationArray, num + 3, len);
        for (int j = (num + 3) + len; j < destinationArray.Length; j++)
        {
            destinationArray[j] = 0;
        }
        byte[] @out = new byte[(len + num) + 10];
        for (int k = 0; k < @out.Length; k += 8)
        {
            code(destinationArray, 0, k, @out, 0, k, key);
        }
        return @out;
    }

    private static uint[] FormatKey(byte[] key)
    {
        if (key.Length == 0)
        {
            throw new ArgumentException("Key must be between 1 and 16 characters in length");
        }
        byte[] destinationArray = new byte[0x10];
        if (key.Length < 0x10)
        {
            Array.Copy(key, 0, destinationArray, 0, key.Length);
            for (int j = key.Length; j < 0x10; j++)
            {
                destinationArray[j] = 0x20;
            }
        }
        else
        {
            Array.Copy(key, 0, destinationArray, 0, 0x10);
        }
        uint[] numArray = new uint[4];
        int num2 = 0;
        for (int i = 0; i < destinationArray.Length; i += 4)
        {
            numArray[num2++] = ConvertByteArrayToUInt(destinationArray, i);
        }
        return numArray;
    }
}

