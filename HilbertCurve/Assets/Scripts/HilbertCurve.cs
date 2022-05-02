using System;
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
            int i = Mathf.RoundToInt(t * NumberOfPoints);
            // TODO for partials, find interpolated value between them
            d2xy(NumberOfPoints, i, out int x1, out int y1);

            return new Vector2(x1 / (float)SquareSize, y1 / (float)SquareSize);
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
