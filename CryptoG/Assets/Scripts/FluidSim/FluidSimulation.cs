using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSimulation : MonoBehaviour
{
    [SerializeField] private int size;
    [SerializeField] private MainBoardController board;

    [SerializeField] private float diff = 1;
    [SerializeField] private float visc = 0.1f;
    [SerializeField] private float iteration = 10;

    private float[] s, density;
    private float[] vX, vY; 
    private float[] vX0, vY0;

    private void Start()
    {
        InitializeBoard();
        InitializeArrays();
    }

    private void InitializeArrays()
    {
        s = new float[size*size];
        density = new float[size*size];

        vX = new float[size*size]; vY = new float[size*size];
        vX0 = new float[size*size]; vY0 = new float[size*size];
    }

    private void InitializeBoard()
    {
        board.SetBoardSize(new Vector2Int(size, size));
        board.board.CreateNewBoard();
        board.pen.RefreshPen();
    }

    #region slowCpuShit
    private int ToInd(int x, int y)
    {
        return x * size + y;
    }
    private void AddDye(float amount, int posX, int posY)
    {
        density[ToInd(posX, posY)] += amount;
    }

    private void AddVelocity(float amX, float amY, int posX, int posY)
    {
        vX[ToInd(posX, posY)] += amX;
        vY[ToInd(posX, posY)] += amY;
    }

    private void LinearSolve(int b, float[] x, float[] x0, float a, float c)
    {
        float cRecip = 1.0f / c;

        for (int k = 0; k < iteration; k++) 
        {
            for (int j = 1; j < size - 1; j++) 
            {
                for (int i = 1; i < size - 1; i++) 
                {
                    x[ToInd(i, j)] =
                        (x0[ToInd(i, j)]
                            + a*(    x[ToInd(i+1, j)]
                                    +x[ToInd(i-1, j)]
                                    +x[ToInd(i  , j+1)]
                                    +x[ToInd(i  , j-1)] 
                                )
                            ) * cRecip;
                }
            }
            //set_bnd(b, x, N);
        }
    }

    private void DiffuseFluid(int b, float[] x, float[] x0)
    {
        float a = Time.deltaTime * diff * (size - 2) * (size - 2);
        LinearSolve(b, x, x0, a, 1 + 6 * a);
    }

    private void ProjectFluid(float[] velocX, float[] velocY, float[] velocZ, float[] p, float[] div)
    {
        for (int j = 1; j < size - 1; j++) 
        {
            for (int i = 1; i < size - 1; i++) 
            {
                div[ToInd(i, j)] = -0.5f*(
                         velocX[ToInd(i+1, j)]
                        -velocX[ToInd(i-1, j)]
                        +velocY[ToInd(i  , j+1)]
                        -velocY[ToInd(i  , j-1)]
                    )/size;
                
                p[ToInd(i, j)] = 0;
            }
        }

        //set_bnd(0, div, N); 
        //set_bnd(0, p, N);
        LinearSolve(0, p, div, 1, 6);
        
        for (int j = 1; j < size - 1; j++) 
        {
            for (int i = 1; i < size - 1; i++) 
            {
                velocX[ToInd(i, j)] -= 0.5f * (  p[ToInd(i+1, j)]
                                                -p[ToInd(i-1, j)]) * size;
                velocY[ToInd(i, j)] -= 0.5f * (  p[ToInd(i, j+1)]
                                                -p[ToInd(i, j-1)]) * size;
            }
        }
        //set_bnd(1, velocX, N);
        //set_bnd(2, velocY, N);
        //set_bnd(3, velocZ, N);
    }
    
    /*static void advect(int b, float[] d, float[] d0,  float[] velocX, float[] velocY, float[] velocZ)
{
    float i0, i1, j0, j1, k0, k1;
    
    float dtx = dt * (N - 2);
    float dty = dt * (N - 2);
    float dtz = dt * (N - 2);
    
    float s0, s1, t0, t1, u0, u1;
    float tmp1, tmp2, tmp3, x, y, z;
    
    float Nfloat = N;
    float ifloat, jfloat, kfloat;
    int i, j, k;
    
    for(k = 1, kfloat = 1; k < N - 1; k++, kfloat++) {
        for(j = 1, jfloat = 1; j < N - 1; j++, jfloat++) { 
            for(i = 1, ifloat = 1; i < N - 1; i++, ifloat++) {
                tmp1 = dtx * velocX[IX(i, j, k)];
                tmp2 = dty * velocY[IX(i, j, k)];
                tmp3 = dtz * velocZ[IX(i, j, k)];
                x    = ifloat - tmp1; 
                y    = jfloat - tmp2;
                z    = kfloat - tmp3;
                
                if(x < 0.5f) x = 0.5f; 
                if(x > Nfloat + 0.5f) x = Nfloat + 0.5f; 
                i0 = floorf(x); 
                i1 = i0 + 1.0f;
                if(y < 0.5f) y = 0.5f; 
                if(y > Nfloat + 0.5f) y = Nfloat + 0.5f; 
                j0 = floorf(y);
                j1 = j0 + 1.0f; 
                if(z < 0.5f) z = 0.5f;
                if(z > Nfloat + 0.5f) z = Nfloat + 0.5f;
                k0 = floorf(z);
                k1 = k0 + 1.0f;
                
                s1 = x - i0; 
                s0 = 1.0f - s1; 
                t1 = y - j0; 
                t0 = 1.0f - t1;
                u1 = z - k0;
                u0 = 1.0f - u1;
                
                int i0i = i0;
                int i1i = i1;
                int j0i = j0;
                int j1i = j1;
                int k0i = k0;
                int k1i = k1;
                
                d[IX(i, j, k)] = 
                
                    s0 * ( t0 * (u0 * d0[IX(i0i, j0i, k0i)]
                                +u1 * d0[IX(i0i, j0i, k1i)])
                        +( t1 * (u0 * d0[IX(i0i, j1i, k0i)]
                                +u1 * d0[IX(i0i, j1i, k1i)])))
                   +s1 * ( t0 * (u0 * d0[IX(i1i, j0i, k0i)]
                                +u1 * d0[IX(i1i, j0i, k1i)])
                        +( t1 * (u0 * d0[IX(i1i, j1i, k0i)]
                                +u1 * d0[IX(i1i, j1i, k1i)])));
            }
        }
    }
    set_bnd(b, d, N);
}*/
    #endregion
}
