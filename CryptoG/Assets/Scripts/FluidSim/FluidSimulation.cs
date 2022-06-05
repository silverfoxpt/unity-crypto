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
    [SerializeField] private float speed = 10f;

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
        x = Mathf.Clamp(x, 0, size-1);
        y = Mathf.Clamp(y, 0, size-1);
        return x*size + y;
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

    private void set_bnd(int b, float[] x)
    {
        for(int i = 1; i < size - 1; i++) 
        {
            x[ToInd(i, 0)] = b == 2 ? -x[ToInd(i, 1)] : x[ToInd(i, 1)];
            x[ToInd(i, size-1)] = b == 2 ? -x[ToInd(i, size-2)] : x[ToInd(i, size-2)];
        }
        
        for(int j = 1; j < size - 1; j++) 
        {
            x[ToInd(0  , j)] = b == 1 ? -x[ToInd(1  , j)] : x[ToInd(1  , j)];
            x[ToInd(size-1, j)] = b == 1 ? -x[ToInd(size-2, j)] : x[ToInd(size-2, j)];
        }

        x[ToInd(0, 0)]       = 0.5f * (x[ToInd(1, 0)]
                                    + x[ToInd(0, 1)]);
        x[ToInd(0, size-1)]     = 0.5f * (x[ToInd(1, size-1)]
                                    + x[ToInd(0, size-2)]);
        x[ToInd(size-1, 0)]     = 0.5f * (x[ToInd(size-2, 0)]
                                    + x[ToInd(size-1, 1)]);
        x[ToInd(size-1, size-1)]   = 0.5f * (x[ToInd(size-2, size-1)]
                                    + x[ToInd(size-1, size-2)]);
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
            set_bnd(b, x);
        }
    }

    private void DiffuseFluid(int b, float[] x, float[] x0, float diff)
    {
        float a = Time.deltaTime * diff * (size - 2) * (size - 2);
        LinearSolve(b, x, x0, a, 1 + 6 * a);
    }

    private void ProjectFluid(float[] velocX, float[] velocY, float[] p, float[] div)
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

        set_bnd(0, div); 
        set_bnd(0, p);
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
        set_bnd(1, velocX);
        set_bnd(2, velocY);
    }
    
    private void AdvectFluid(int b, float[] d, float[] d0,  float[] velocX, float[] velocY)
    {
        float i0, i1, j0, j1;
        
        float dtx = Time.deltaTime * (size - 2);
        float dty = Time.deltaTime * (size - 2);
        
        float s0, s1, t0, t1;
        float tmp1, tmp2, x, y;
        
        float Nfloat = size;
        float ifloat, jfloat;
        int i, j;
        
        for (j = 1, jfloat = 1; j < size - 1; j++, jfloat++) 
        { 
            for (i = 1, ifloat = 1; i < size - 1; i++, ifloat++) 
            {
                tmp1 = dtx * velocX[ToInd(i, j)];
                tmp2 = dty * velocY[ToInd(i, j)];
                x    = ifloat - tmp1; 
                y    = jfloat - tmp2;
                
                if(x < 0.5f) x = 0.5f; 
                if(x > Nfloat + 0.5f) x = Nfloat + 0.5f; 
                i0 = Mathf.Floor(x); 
                i1 = i0 + 1.0f;

                if(y < 0.5f) y = 0.5f; 
                if(y > Nfloat + 0.5f) y = Nfloat + 0.5f; 
                j0 = Mathf.Floor(y);
                j1 = j0 + 1.0f; 
                
                s1 = x - i0; 
                s0 = 1.0f - s1; 
                t1 = y - j0; 
                t0 = 1.0f - t1;
                
                int i0i = (int) i0;
                int i1i = (int) i1;
                int j0i = (int) j0;
                int j1i = (int) j1;
                
                d[ToInd(i, j)] = 
                
                    s0 * ( t0 * d0[ToInd(i0i, j0i)]
                        + t1 * d0[ToInd(i0i, j1i)])

                +   s1 * ( t0 * d0[ToInd(i1i, j0i)]
                        + t1 * d0[ToInd(i1i, j1i)]);
            }
        }
    
        set_bnd(b, d);
    }
    
    private void Update()
    {
        AddFluid();
        UpdateFluid();
        DisplayBoard();
    }

    private void AddFluid()
    {
        for (int i = size/2-2; i <= size/2+2; i++)
        {
            for (int j = size/2-2; j <= size/2+2; j++)
            {
                AddDye(1f, i, j);
                Vector2 v = UnityEngine.Random.insideUnitCircle.normalized * speed;
                AddVelocity(v.x, v.y, i, j); 
            }
        }
    }

    private void DisplayBoard()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float v = Mathf.Clamp(density[ToInd(i, j)], 0f, 1f); //if (v > 0.5f) {Debug.Log(new Vector2Int(i, j));}
                Color col = new Color(v, v, v, 1f);
                board.board.SetPixelDirect(new Vector2Int(i, j), col, false);
            }
        }
        board.board.imageTex.Apply();
    }

    private void UpdateFluid()
    {
        DiffuseFluid(1, vX0, vX, visc);
        DiffuseFluid(2, vY0, vY, visc);

        ProjectFluid(vX0, vY0, vX, vY);

        AdvectFluid(1, vX, vX0, vX0, vY0);
        AdvectFluid(2, vY, vY0, vX0, vY0);

        ProjectFluid(vX, vY, vX0, vY0);

        DiffuseFluid(0, s, density, diff);
        AdvectFluid(0, density, s, vX, vY);
    }
    #endregion
}
