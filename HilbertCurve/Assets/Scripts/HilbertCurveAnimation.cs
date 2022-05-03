using Runamuck;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Runamuck
{
    public class HilbertCurveAnimation : MonoBehaviour
    {
        [SerializeField] private Vector2 size = new Vector2(10, 10);
        [SerializeField] private HilbertCurve curve = new HilbertCurve();

        [SerializeField] private GameObject baseEntityPrefab;
        [SerializeField] private int entityCount = 10;
        [SerializeField] private float animationSpeed = 1 / 5f;

        public class AnimatedObject
        {
            public float t;
            public Transform transform;
        }
        private List<AnimatedObject> animatedObjects = new List<AnimatedObject>();

        // TODO
        // - Spawn objects that follow the path
        // - Figure out a good way to infinite loop
        // - Test video capture with Unity
        // - Look into scaling this massively with ECS
        // - Make something visually intriquing
        void Start()
        {
            DOTween.Init(true, false);

            for (int i = 0; i < entityCount; i++)
            {
                float baseT = i / (float)entityCount;
                Vector3 pos1 = GetPosition(baseT);
                var go = Instantiate(baseEntityPrefab, pos1, Quaternion.identity);
                var animatedObject = new AnimatedObject
                {
                    t = i / (float)entityCount,
                    transform = go.transform
                };
                animatedObjects.Add(animatedObject);
                Transform entityTransform = go.transform;

                Sequence sequence = DOTween.Sequence();

                float d1 = 1 - baseT;
                if(d1 != 0)
                {
                    sequence.Append(
                    DOTween.To(() => animatedObject.t, t =>
                    {
                        animatedObject.t = t;
                        animatedObject.transform.position = GetPosition(t);
                    }, 1, d1 / animationSpeed));
                }

                if(baseT != 0)
                {
                    sequence.AppendCallback(() =>
                    {
                        animatedObject.t = 0;
                        animatedObject.transform.position = GetPosition(0);
                    });
                    sequence.Append(
                    DOTween.To(() => animatedObject.t, t =>
                    {
                        animatedObject.t = t;
                        animatedObject.transform.position = GetPosition(t);
                    }, baseT, 1 / animationSpeed));
                }
            }

        }

        private Vector3 GetPosition(float t)
        {
            Vector2 pos = curve.GetPosition(t);
            pos *= size;

            return new Vector3(pos.x, pos.y, 0);
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
}
