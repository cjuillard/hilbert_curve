using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : MonoBehaviour
{
    public Transform[] displays;

    public int iterations = 100;

    public void SetTransformT(float t)
    {
        t = (t * iterations) % 1;
        t %= 1;

        if(displays.Length == 0)
        {
            return;
        }

        float tIndex = t * displays.Length;
        for(int i = 0; i < displays.Length; i++)
        {
            float currDist = Mathf.Abs(i - tIndex);
            if(i == 0)
            {
                float wrapDist = Mathf.Abs(tIndex - displays.Length);
                currDist = Mathf.Min(wrapDist, currDist);
            }
            currDist = Mathf.Min(1, currDist);
            float scale = (1 - currDist);
            displays[i].localScale = new Vector3(scale, scale, scale);
        }


    }
}
