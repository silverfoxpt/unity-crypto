using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD5Hash : MonoBehaviour
{
    [SerializeField] private string inputString = "Hello!";

    private int[] shift = {
        7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
        5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
        4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
        6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,
    };

    private uint[] kConst = {
        0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
        0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
        0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
        0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,

        0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
        0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
        0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
        0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,

        0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
        0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
        0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
        0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,

        0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
        0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
        0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
        0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391,
    };

    private const uint a0 = 0x67452301, b0 = 0xefcdab89, c0 = 0x98badcfe, d0 = 0x10325476;

    void Start()
    {
        //Debug.Log(MD5(inputString));
    }

    /// <summary>
    /// Return a MD5 string in hex from raw input string
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns>A string, hashed with MD5 algorithm</returns>
    private string MD5(string inputString)
    {
        string paddedInput = PadInput(inputString); 
        List<List<uint>> choppedBlocks = GetChoppedBlock(paddedInput);

        uint aMain = a0, bMain = b0, cMain = c0, dMain = d0;
        foreach(List<uint> block in choppedBlocks)
        {
            uint A = aMain, B = bMain, C = cMain, D = dMain;

            for (int i = 0; i < 64; i++)
            {
                uint F; int g;
                if (0 <= i && i <= 15)
                {
                    F = (B & C) | ((~B) & D);
                    g = i;
                }
                else if (16 <= i && i <= 31)
                {
                    F = (B & D) | (C & (~D));
                    g = (i*5+1) %16;
                }
                else if (32 <= i && i <= 47)
                {
                    F = B ^ C ^ D;
                    g = (i*3+5) %16;
                }
                else
                { 
                    F = C ^ (B | (~D));
                    g = (i*7) %16;
                }

                F = F + A + kConst[i] + block[g];  
                A = D; D = C; C = B; 
                B = B + UtilityFunc.RotateBitLeft(F, shift[i]);
            }
            aMain += A;
            bMain += B;
            cMain += C; 
            dMain += D;
        }

        string res = "";
        res += UtilityFunc.BinaryToHex(UtilityFunc.BinaryToLittleEndian(UtilityFunc.UintToBinary(aMain, 32))); 
        res += UtilityFunc.BinaryToHex(UtilityFunc.BinaryToLittleEndian(UtilityFunc.UintToBinary(bMain, 32)));
        res += UtilityFunc.BinaryToHex(UtilityFunc.BinaryToLittleEndian(UtilityFunc.UintToBinary(cMain, 32)));
        res += UtilityFunc.BinaryToHex(UtilityFunc.BinaryToLittleEndian(UtilityFunc.UintToBinary(dMain, 32)));
        return res.ToLower();
    }

    /// <summary>
    /// Chops input into 512-bit blocks, each block have 16 uint (unsigned integer) -> 32-bit. Process in little-endian.
    /// </summary>
    /// <param name="inp"></param>
    /// <returns>List of blocks, each block is a List of uint</returns>
    private List<List<uint>> GetChoppedBlock(string inp)
    {
        List<List<uint>> chopped = new List<List<uint>>();

        int counter = 0;
        for (int idx = 0; idx < inp.Length; idx += 512)
        {
            chopped.Add(new List<uint>());
            for (int j = idx; j < idx + 512; j += 32)
            {
                string a1 = "", a2 = "", a3 = "", a4 = "";

                //stupid maneuver to convert to little endian
                for (int k = j; k < j + 8; k++) {a1 += inp[k];}
                for (int k = j+8; k < j + 16; k++) {a2 += inp[k];}
                for (int k = j+16; k < j + 24; k++) {a3 += inp[k];}
                for (int k = j+24; k < j + 32; k++) {a4 += inp[k];}

                chopped[counter].Add(UtilityFunc.BinaryToUint(a4+a3+a2+a1));
            }
            counter++;
        }

        return chopped;
    }

    /// <summary>
    /// Pad the raw input MD5 string
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns>A string, which length is divisible by 512</returns>
    private string PadInput(string inputString)
    {
        string bin = "";
        foreach (var c in inputString)
        {
            string binChar = UtilityFunc.ReverseString(UtilityFunc.UintToBinary(c, 8));
            while (binChar.Length > 8) { binChar = binChar.Remove(binChar.Length - 1); }
            while (binChar.Length < 8) { binChar = binChar + '0'; }

            bin += UtilityFunc.ReverseString(binChar);
        }

        int remains = bin.Length % 512; uint oldLen = (uint) bin.Length;

        // padding
        bin += '1'; //pad 1
        if (remains < 448)
        {
            while (bin.Length % 512 < 448) { bin += '0'; }
        }
        else
        {
            int neededSecondLastBlock = 512 - bin.Length;
            for (int idx = 0; idx < neededSecondLastBlock + 448; idx++) {bin += '0';}
        }
        bin += GetBinaryLength(oldLen);

        return bin;
    }

    /// <summary>
    /// Return length of MD5 string in binary with length of 64 bit (longer length will be stripped down to 64, shorter will be padded with 0). 
    /// Note: Return binary string is in little-endian.
    /// </summary>
    /// <param name="len">Length of MD5 string (before padding)</param>
    /// <returns>A binary string with length of exactly 64 bit</returns>
    private string GetBinaryLength(uint len)
    {
        string bin = UtilityFunc.UintToBinary(len);  

        int neededPadding = 8* Mathf.CeilToInt(bin.Length/8f); 
        while(bin.Length < neededPadding) {bin = '0' + bin;} 

        List<string> blocks = new List<string>();
        for (int idx = 0; idx < bin.Length; idx+=8)
        {
            string a = "";
            for (int j = idx; j < idx+8; j++) { a += bin[j];}
            blocks.Add(a);
        }

        bin = "";
        for (int idx = blocks.Count - 1; idx >= 0; idx--) { bin += blocks[idx]; }

        while (bin.Length < 64) {bin += '0';} return bin;
    }
}

