using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA1Hash : MonoBehaviour
{
    [SerializeField] private string userInput;

    private const uint h0 = 0x67452301, h1 = 0xefcdab89, h2 = 0x98badcfe, h3 = 0x10325476, h4 = 0xc3d2e1f0;
    private uint ml = 0;

    void Start()
    {
        //Debug.Log(SHA1(userInput));
    }

    /// <summary>
    /// Apply SHA-1 hash algorithm to a raw ASCII input string
    /// </summary>
    /// <param name="input"></param>
    /// <returns>SHA-1 hash of the input string, length 160 bits</returns>
    private string SHA1(string input)
    {
        ml = GetInputLength(input);
        List<List<uint>> blocks = GetChoppedBlocks(GetPaddedInput(input));

        //hash
        uint aMain = h0, bMain = h1, cMain = h2, dMain = h3, eMain = h4;

        foreach(var blockies in blocks)
        {
            List<uint> block = new List<uint>(blockies); 
            for (int i = 16; i < 80; i++)
            {
                block.Add(UtilityFunc.RotateBitLeft(block[i-3] ^ block[i-8] ^ block[i-14] ^ block[i-16], 1));
            }

            uint A = h0, B = h1, C = h2, D = h3, E = h4; 
            for (int i = 0; i < 80; i++)
            {
                uint F, K;
                if (0 <= i && i <= 19)
                {
                    F = (B & C) | ((~B) & D);
                    K = 0x5a827999;
                }
                else if (20 <= i && i <= 39)
                {
                    F = B ^ C ^ D;
                    K = 0x6ed9eba1;
                }
                else if (40 <= i && i <= 59)
                {
                    F = (B & C) | (B & D) | (C & D);
                    K = 0x8f1bbcdc;
                }
                else
                {
                    F = B ^ C ^ D;
                    K = 0xca62c1d6;
                }
                
                uint tmp = UtilityFunc.RotateBitLeft(A, 5) + F + E + K + block[i]; 
                E = D; D = C; C = UtilityFunc.RotateBitLeft(B, 30); B = A; A = tmp; 
            }
            aMain += A; bMain += B; cMain += C; dMain += D; eMain += E;
        }

        string hexA = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(aMain));
        string hexB = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(bMain));
        string hexC = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(cMain));
        string hexD = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(dMain));
        string hexE = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(eMain));
        return (hexA + hexB + hexC + hexD + hexE).ToLower();
    }

    /// <summary>
    /// Chop a padded string into blocks of 512 bits each
    /// </summary>
    /// <param name="input"></param>
    /// <returns>List of list of 16 unsigned integers (32-bit) in a block</returns>
    private List<List<uint>> GetChoppedBlocks(string input)
    {
        List<List<uint>> res = new List<List<uint>>();

        int counter = 0;
        for (int i = 0; i < input.Length; i+=512)
        {
            res.Add(new List<uint>());
            for (int j = i; j < i+512; j+=32)
            {
                string bin = "";
                for (int k = j; k < j+32; k++) {  bin += input[k];}
                res[counter].Add(UtilityFunc.BinaryToUint(bin));
            }
            counter++;
        }
        return res;
    }

    /// <summary>
    /// Pad the input of the SHA-1 algorithm, padded to reach length divisible by 512
    /// </summary>
    /// <param name="input"></param>
    /// <returns>The input string</returns>
    private string GetPaddedInput(string input)
    {
        string bin = GetBinaryInput(input);
        
        //padding
        bin += '1';
        while(bin.Length % 512 != 448) {bin += '0';}

        bin += UtilityFunc.UintToBinary(ml, 64);
        return bin;
    }

    /// <summary>
    /// Get binary string of a raw ASCII string
    /// </summary>
    /// <param name="input"></param>
    /// <returns>A binary string of the input string</returns>
    private string GetBinaryInput(string input)
    {
        string bin = "";
        foreach(var c in input)
        {
            bin += UtilityFunc.UintToBinary((uint) c, 8);
        }
        return bin;
    }

    /// <summary>
    /// Return length in bits of a string
    /// </summary>
    /// <param name="input"></param>
    /// <returns>An unsigned integer (32-bit) indicating the length of the input string</returns>
    private uint GetInputLength(string input)
    {
        return (uint) input.Length * 8;
    }
}
