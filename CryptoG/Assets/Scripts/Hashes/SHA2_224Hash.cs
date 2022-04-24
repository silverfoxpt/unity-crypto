using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA2_224Hash : MonoBehaviour
{
    [SerializeField] private string userInput = "";
    private const uint h0 = 0xc1059ed8;
    private const uint h1 = 0x367cd507;
    private const uint h2 = 0x3070dd17;
    private const uint h3 = 0xf70e5939;
    private const uint h4 = 0xffc00b31;
    private const uint h5 = 0x68581511;
    private const uint h6 = 0x64f98fa7;
    private const uint h7 = 0xbefa4fa4;

    private uint[] kConst = {
        0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
        0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
        0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
        0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
        0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
        0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
        0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
        0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
    };

    void Start()
    {
        Debug.Log(SHA2_224(userInput));
    }

    private string SHA2_224(string input)
    {
        List<List<uint>> blocks = GetChoppedBlocks(GetPaddedInput(input));

        uint aMain = h0, bMain = h1, cMain = h2, dMain = h3;
        uint eMain = h4, fMain = h5, gMain = h6, hMain = h7;
        foreach(var blockies in blocks)
        {
            uint A = h0, B = h1, C = h2, D = h3;
            uint E = h4, F = h5, G = h6, H = h7;

            List<uint> block = new List<uint>(blockies);
            for (int i = 16; i < 64; i++) //added adequate number of uint
            {
                uint s0 = UtilityFunc.RotateBitRight(block[i-15], 7) ^ UtilityFunc.RotateBitRight(block[i-15], 18)  ^ (block[i-15] >> 3);
                uint s1 = UtilityFunc.RotateBitRight(block[i-2], 17) ^ UtilityFunc.RotateBitRight(block[i-2], 19)   ^ (block[i-2] >> 10);
                block.Add(block[i-16] + s0 + block[i-7] + s1);
            }

            //hash
            for (int i = 0; i < 64; i++)
            {
                uint s1 = UtilityFunc.RotateBitRight(E, 6) ^ UtilityFunc.RotateBitRight(E, 11) ^ UtilityFunc.RotateBitRight(E, 25);
                uint ch =  (E & F) ^ ((~E) & G);
                uint tmp1 =  H + s1 + ch + kConst[i] + block[i];

                uint s0 = UtilityFunc.RotateBitRight(A, 2) ^ UtilityFunc.RotateBitRight(A, 13) ^ UtilityFunc.RotateBitRight(A, 22);
                uint maj = (A & B) ^ (A & C) ^ (B & C);
                uint tmp2 = s0 + maj;

                H = G; G = F; F = E;
                E = D + tmp1;
                D = C; C = B; B = A;
                A = tmp1 + tmp2;
            }
            aMain += A;
            bMain += B;
            cMain += C;
            dMain += D;
            eMain += E;
            fMain += F;
            gMain += G;
            hMain += H;
        }

        string hexA = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(aMain));
        string hexB = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(bMain));
        string hexC = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(cMain));
        string hexD = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(dMain));
        string hexE = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(eMain));
        string hexF = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(fMain));
        string hexG = UtilityFunc.BinaryToHex(UtilityFunc.UintToBinary(gMain));
        return (hexA + hexB + hexC + hexD + hexE + hexF + hexG).ToLower();
    }

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

    private string GetPaddedInput(string input)
    {
        string bin = "";
        foreach (var c in input)
        {
            bin += UtilityFunc.UintToBinary((uint) c, 8);
        }

        //padding
        bin += '1';
        while(bin.Length % 512 != 448) {bin += '0';}
        bin += UtilityFunc.UintToBinary((uint) (input.Length * 8), 64); //append length

        return bin;
    }
}

