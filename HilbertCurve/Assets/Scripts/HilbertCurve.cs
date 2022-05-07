using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    [Serializable]
    public class HilbertCurve
    {
        [Range(1, 8)]
        [SerializeField] private int numberOfSplits = 1;
        public int SquareSize { get; private set; } = 2;
        public int NumberOfPoints { get; private set; } = 4;

        public HilbertCurve(int numberOfSplits = 1)
        {
            this.numberOfSplits = numberOfSplits;
        }

        public void UpdateParams()
        {
            SquareSize = (int)Mathf.Pow(2, numberOfSplits);
            NumberOfPoints = SquareSize * SquareSize;
        }

        public int NumberOfSplits
        {
            get => numberOfSplits;
            set
            {
                numberOfSplits = Math.Max(1, value);
            }
        }

        public Vector2 GetPosition(float t)
        {
            float pointNum = t * (NumberOfPoints - 1);
            int i1 = Mathf.FloorToInt(pointNum);
            int i2 = Mathf.CeilToInt(pointNum);

            d2xy(NumberOfPoints, i1, out int x1, out int y1);
            d2xy(NumberOfPoints, i2, out int x2, out int y2);

            return new Vector2(Mathf.Lerp(x1, x2, pointNum - i1) / (float)SquareSize,
                Mathf.Lerp(y1, y2, pointNum - i1) / (float)SquareSize);
        }

        public Vector2 GetBezierPosition(float t)
        {
            Vector2 pos = GetBezierPosition(t, out _, out _, out _);
            pos *= 1f / SquareSize;
            return pos;
        }

        public Vector2 GetBezierPosition(float t, out Vector2 a, out Vector2 b, out Vector2 c)
        {
            float pointNum = t * (NumberOfPoints - 1);
            int i1 = Mathf.FloorToInt(pointNum);
            int i2 = Mathf.CeilToInt(pointNum);

            if (i1 == i2)
            {
                i2 = Math.Min(NumberOfPoints - 1, i1 + 1);
            }

            d2xy(NumberOfPoints, i1, out int x1, out int y1);
            d2xy(NumberOfPoints, i2, out int x2, out int y2);

            Vector2 p0, p1, p2;
            float normalizedT;
            if (pointNum - i1 > 0.5f)
            {
                int i3 = Math.Min(NumberOfPoints - 1, i2 + 1);
                if(i3 == i2)    // Handle edge case for end of the curve
                {
                    p0 = new Vector2(x1, y1);
                    p1 = new Vector2(0.5f * (x1 + x2), 0.5f * (y1 + y2));
                    p2 = new Vector2(x2, y2);
                    normalizedT = (pointNum - i1) * 2f - 0.5f;
                }
                else
                {
                    d2xy(NumberOfPoints, i3, out int x3, out int y3);
                    p0 = new Vector2(x1, y1);
                    p1 = new Vector2(x2, y2);
                    p2 = new Vector2(x3, y3);
                    normalizedT = (pointNum - i1) - 0.5f;
                }
                
            }
            else
            {
                int i3 = Math.Max(0, i1 - 1);
                if(i3 == i1)    // Edge case for start of the curve
                {
                    p0 = new Vector2(x1, y1);
                    p1 = new Vector2(0.5f * (x1+x2), 0.5f * (y1+y2));
                    p2 = new Vector2(x2, y2);
                    normalizedT = (pointNum - i3) * 2f - 0.5f;
                }
                else
                {
                    d2xy(NumberOfPoints, i3, out int x3, out int y3);
                    p0 = new Vector2(x3, y3);
                    p1 = new Vector2(x1, y1);
                    p2 = new Vector2(x2, y2);
                    normalizedT = (pointNum - i3) - 0.5f;
                }
                
                
            }

            Vector2 interp0 = 0.5f * (p0 + p1);
            Vector2 interp1 = 0.5f * (p2 + p1);

            a = p0;
            b = p1;
            c = p2;
            //Debug.Log($"NormalizeT={normalizedT}");
            return GetCubicBezier(normalizedT, interp0, p1, interp1);
        }

        private Vector2 GetCubicBezier(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return p1 + (1 - t) * (1 - t) * (p0 - p1) + t * t * (p2 - p1);
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

}
