using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHA2_512Hash : MonoBehaviour
{
    [SerializeField] private string userInput = "";
    private const ulong h0 = 0x6a09e667f3bcc908;
    private const ulong h1 = 0xbb67ae8584caa73b;
    private const ulong h2 = 0x3c6ef372fe94f82b;
    private const ulong h3 = 0xa54ff53a5f1d36f1;
    private const ulong h4 = 0x510e527fade682d1;
    private const ulong h5 = 0x9b05688c2b3e6c1f;
    private const ulong h6 = 0x1f83d9abfb41bd6b;
    private const ulong h7 = 0x5be0cd19137e2179;

    private ulong[] kConst = {
        0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc, 0x3956c25bf348b538, 
        0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118, 0xd807aa98a3030242, 0x12835b0145706fbe, 
        0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2, 0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235, 
        0xc19bf174cf692694, 0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65, 
        0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5, 0x983e5152ee66dfab, 
        0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4, 0xc6e00bf33da88fc2, 0xd5a79147930aa725, 
        0x06ca6351e003826f, 0x142929670a0e6e70, 0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed, 
        0x53380d139d95b3df, 0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b, 
        0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30, 0xd192e819d6ef5218, 
        0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8, 0x19a4c116b8d2d0c8, 0x1e376c085141ab53, 
        0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8, 0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373, 
        0x682e6ff3d6b2b8a3, 0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec, 
        0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b, 0xca273eceea26619c, 
        0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178, 0x06f067aa72176fba, 0x0a637dc5a2c898a6, 
        0x113f9804bef90dae, 0x1b710b35131c471b, 0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc, 
        0x431d67c49c100d4c, 0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817,
    };

    void Start()
    {
        Debug.Log(SHA2_512(userInput));
    }

    private string SHA2_512(string input)
    {
        List<List<ulong>> blocks = GetChoppedBlocks(GetPaddedInput(input));

        ulong aMain = h0, bMain = h1, cMain = h2, dMain = h3;
        ulong eMain = h4, fMain = h5, gMain = h6, hMain = h7;
        foreach(var blockies in blocks)
        {
            ulong A = h0, B = h1, C = h2, D = h3;
            ulong E = h4, F = h5, G = h6, H = h7;

            List<ulong> block = new List<ulong>(blockies);
            for (int i = 16; i < 80; i++) //added adequate number of ulong
            {
                ulong s0 = UtilityFunc.RotateBitRightUlong(block[i-15], 1) ^ UtilityFunc.RotateBitRightUlong(block[i-15], 8)  ^ (block[i-15] >> 7);
                ulong s1 = UtilityFunc.RotateBitRightUlong(block[i-2], 19) ^ UtilityFunc.RotateBitRightUlong(block[i-2], 61)   ^ (block[i-2] >> 6);
                block.Add(block[i-16] + s0 + block[i-7] + s1);
            }
            Debug.Log(block.Count);

            //hash
            for (int i = 0; i < 80; i++)
            {
                ulong s1 = UtilityFunc.RotateBitRightUlong(E, 14) ^ UtilityFunc.RotateBitRightUlong(E, 18) ^ UtilityFunc.RotateBitRightUlong(E, 41);
                ulong ch =  (E & F) ^ ((~E) & G);
                ulong tmp1 =  H + s1 + ch + kConst[i] + block[i];

                ulong s0 = UtilityFunc.RotateBitRightUlong(A, 28) ^ UtilityFunc.RotateBitRightUlong(A, 34) ^ UtilityFunc.RotateBitRightUlong(A, 39);
                ulong maj = (A & B) ^ (A & C) ^ (B & C);
                ulong tmp2 = s0 + maj;

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

        string hexA = UtilityFunc.BinaryToHex(UtilityFunc.UlongToBinary(aMain));
        string hexB = UtilityFunc.BinaryToHex(UtilityFunc.UlongToBinary(bMain));
        string hexC = UtilityFunc.BinaryToHex(UtilityFunc.UlongToBinary(cMain));
        string hexD = UtilityFunc.BinaryToHex(UtilityFunc.UlongToBinary(dMain));
        string hexE = UtilityFunc.BinaryToHex(UtilityFunc.UlongToBinary(eMain));
        string hexF = UtilityFunc.BinaryToHex(UtilityFunc.UlongToBinary(fMain));
        string hexG = UtilityFunc.BinaryToHex(UtilityFunc.UlongToBinary(gMain));
        string hexH = UtilityFunc.BinaryToHex(UtilityFunc.UlongToBinary(hMain));
        return (hexA + hexB + hexC + hexD + hexE + hexF + hexG + hexH).ToLower();
    }

    private List<List<ulong>> GetChoppedBlocks(string input)
    {
        List<List<ulong>> res = new List<List<ulong>>();

        int counter = 0;
        for (int i = 0; i < input.Length; i+=1024)
        {
            res.Add(new List<ulong>());
            for (int j = i; j < i+1024; j+=64)
            {
                string bin = "";
                for (int k = j; k < j+64; k++) { bin += input[k]; }
                res[counter].Add(UtilityFunc.BinaryToUlong(bin));
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
            bin += UtilityFunc.UlongToBinary((ulong) c, 8);
        }

        //padding
        bin += '1';
        while(bin.Length % 1024 != 896) {bin += '0';}
        bin += UtilityFunc.UlongToBinary((ulong) (input.Length * 8), 128); //append length

        return bin;
    }
}

