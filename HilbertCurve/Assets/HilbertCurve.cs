using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HilbertCurve : MonoBehaviour
{
    [Range(1, 100)]
    public int splits = 1;
    public Vector2 size = new Vector2(1, 1);

    // Start is called before the first frame update
    void Start()
    {
        Gizmos.DrawSphere(transform.position, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;

        int squareSize = (int)Mathf.Pow(2, splits);
        Vector2 scale = new Vector2(size.x / (float)squareSize, size.y / (float)squareSize);
        for(int i = 0, n = squareSize * squareSize; i < n - 1; i++)
        {
            d2xy(n, i, out int x1, out int y1);
            d2xy(n, i+1, out int x2, out int y2);
            Gizmos.DrawLine(new Vector3(x1 * scale.x, y1 * scale.y, 0), new Vector3(x2 * scale.x, y2 * scale.y, 0));
        }
        Gizmos.DrawSphere(transform.position, .1f);
    }

    ////convert (x,y) to d
    //int xy2d(int n, int x, int y)
    //{
    //    int rx, ry, s, d = 0;
    //    for (s = n / 2; s > 0; s /= 2)
    //    {
    //        rx = (x & s) > 0;
    //        ry = (y & s) > 0;
    //        d += s * s * ((3 * rx) ^ ry);
    //        rot(n, &x, &y, rx, ry);
    //    }
    //    return d;
    //}

    // Code ported from C version on Wikipedia - https://en.wikipedia.org/wiki/Hilbert_curve
    //It assumes a square divided into n by n cells, for n a power of 2, with integer coordinates, with (0,0) in the lower left corner,
    // (n − 1, n − 1) in the upper right corner, and a distance d that starts at 0 in the lower left corner and goes to n^2 - 1 in the lower-right corner. 

    //convert d to (x,y)
    void d2xy(int n, int d, out int x, out int y)
    {
        int rx, ry, s, t = d;
        x = y = 0;
        for (s = 1; s < n; s *= 2)
        {
            rx = 1 & (t / 2);
            ry = 1 & (t ^ rx);
            rot(s, ref x, ref y, rx, ry);
            x += s * rx;
            y += s * ry;
            t /= 4;
        }
    }

    //rotate/flip a quadrant appropriately
    void rot(int n, ref int x, ref int y, int rx, int ry)
    {
        if (ry == 0)
        {
            if (rx == 1)
            {
                x = n - 1 - x;
                y = n - 1 - y;
            }

            //Swap x and y
            int t = x;
            x = y;
            y = t;
        }
    }
}
