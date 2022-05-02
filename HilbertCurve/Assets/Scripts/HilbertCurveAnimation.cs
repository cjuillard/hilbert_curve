using Runamuck;
using UnityEngine;

public class HilbertCurveAnimation : MonoBehaviour
{
    public Vector2 size = new Vector2(10, 10);
    public HilbertCurve curve = new HilbertCurve();
    
    // Start is called before the first frame update
    void Start()
    {
        Gizmos.DrawSphere(transform.position, 1);
    }

    private void OnValidate()
    {
        curve.UpdateParams();
    }

    void Update()
    {

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector2 scale = new Vector2(size.x / (float)curve.SquareSize, size.y / (float)curve.SquareSize);
        for (int i = 0, n = curve.NumberOfPoints; i < n - 1; i++)
        {
            Vector2 p1 = curve.GetPosition(i / (float)n);
            Vector2 p2 = curve.GetPosition((i + 1) / (float)n);
            
            Gizmos.DrawLine(new Vector3(p1.x * size.x, p1.y * size.y, 0), new Vector3(p2.x * size.x, p2.y * size.y, 0));
        }
        Gizmos.DrawSphere(transform.position, .1f);
    }
}
