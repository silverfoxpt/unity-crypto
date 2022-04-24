using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD4Hash : MonoBehaviour
{
    //[SerializeField] private string inputString = "Hello!";

    private int[] shift = {
        3, 7, 11, 19,  3, 7, 11, 19,  3, 7, 11, 19,  3, 7, 11, 19,  
        3, 5, 9, 13,   3, 5, 9, 13,   3, 5, 9, 13,   3, 5, 9, 13,   
        3, 9, 11, 15,  3, 9, 11, 15,  3, 9, 11, 15,  3, 9, 11, 15,  
    };

    private int[] convertCustom = {
        0, 8, 4, 12,  2, 10, 6, 14,  1, 9, 5, 13,  3, 11, 7, 15, 
    };

    private int[] convertCustom2 = {
        0, 4, 8, 12,  1, 5, 9, 13,  2, 6, 10, 14,  3, 7, 11, 15
    };

    private const uint a0 = 0x67452301, b0 = 0xefcdab89, c0 = 0x98badcfe, d0 = 0x10325476;

    void Start()
    {
        //Debug.Log(MD4(inputString));
    }

    /// <summary>
    /// Return a MD4 string in hex from raw input string
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns>A string, hashed with MD4 algorithm</returns>
    private string MD4(string inputString)
    {
        string paddedInput = PadInput(inputString); 
        List<List<uint>> choppedBlocks = GetChoppedBlock(paddedInput);

        uint aMain = a0, bMain = b0, cMain = c0, dMain = d0;
        foreach(List<uint> block in choppedBlocks)
        {
            uint A = aMain, B = bMain, C = cMain, D = dMain;

            for (int i = 0; i < 48; i++)
            {
                uint F, kConst; int g; 
                if (0 <= i && i <= 15)
                {
                    F = (B & C) | ((~B) & D);
                    g = i; kConst = 0;
                }
                else if (16 <= i && i <= 31)
                {
                    F = (((B & C) | (B & D)) | (C & D));
                    g = convertCustom2[i%16]; kConst = 0x5a827999;
                }
                else 
                {
                    F = B ^ C ^ D;
                    g = convertCustom[i%16]; kConst = 0x6ed9eba1;
                }

                F = F + A + kConst + block[g];  
                A = D; D = C; C = B; 
                B = UtilityFunc.RotateBitLeft(F, shift[i]);
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
    /// Return length of MD4 string in binary with length of 64 bit (longer length will be stripped down to 64, shorter will be padded with 0). 
    /// Note: Return binary string is in little-endian.
    /// </summary>
    /// <param name="len">Length of MD4 string (before padding)</param>
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


