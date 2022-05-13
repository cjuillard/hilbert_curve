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
            public Transformer transformer;

            public virtual void SetT(float t)
            {
                this.t = t;
                if (transformer != null)
                    transformer.SetTransformT(t);
            }
        }

        private List<AnimatedObject> animatedObjects = new List<AnimatedObject>();

        void Start()
        {
            DOTween.Init(true, false);

            for (int i = 0; i < entityCount; i++)
            {
                float baseT = i / (float)entityCount;
                Vector3 pos1 = GetPosition(baseT);
                var go = Instantiate(baseEntityPrefab, pos1, Quaternion.identity, transform);
                var animatedObject = new AnimatedObject
                {
                    t = i / (float)entityCount,
                    transform = go.transform,
                    transformer = go.GetComponent<Transformer>()
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
                        animatedObject.SetT(t);
                        animatedObject.transform.localPosition = GetPosition(t);
                        animatedObject.transform.rotation = GetRotation(t);
                    }, 1, d1 / animationSpeed)
                    .SetEase(Ease.Linear));
                }

                if(baseT != 0)
                {
                    sequence.AppendCallback(() =>
                    {
                        animatedObject.SetT(0);
                        animatedObject.transform.localPosition = GetPosition(0);
                        animatedObject.transform.rotation = GetRotation(0);
                    });
                    sequence.Append(
                    DOTween.To(() => animatedObject.t, t =>
                    {
                        animatedObject.SetT(t);
                        animatedObject.transform.localPosition = GetPosition(t);
                        animatedObject.transform.rotation = GetRotation(t);
                    }, baseT, baseT / animationSpeed)
                    .SetEase(Ease.Linear));
                }
            }

        }

        public Vector3 rotationSpeed = new Vector3(10, 6, 8);
        private Quaternion GetRotation(float t)
        {
            Vector3 eulerAngles = new Vector3(t * 360 * rotationSpeed.x,
                t * 360 * rotationSpeed.y + 59,
                t * 360 * rotationSpeed.z + 35);
            return Quaternion.Euler(eulerAngles);
        }

        private Vector3 GetPositionOnHilbertCurve(float t)
        {
            Vector2 pos = curve.GetPosition(t);
            pos *= size;

            return new Vector3(pos.x, pos.y, 0);
        }
        private Vector3 GetPosition(float t)
        {
            Vector2 pos = curve.GetBezierPosition(t);

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

        private Vector3 NormalizedToWorldPos(Vector2 p)
        {
            p *= size;
            Vector3 v3 = new Vector3(p.x, p.y, 0);
            v3 = transform.localToWorldMatrix.MultiplyPoint(v3);
            return v3;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Vector2 scale = new Vector2(size.x / (float)curve.SquareSize, size.y / (float)curve.SquareSize);
            for (int i = 0, n = curve.NumberOfPoints; i < n - 1; i++)
            {
                Vector3 p1 = GetPosition(i / (float)n);
                Vector3 p2 = GetPosition((i + 1) / (float)n);
                p1 = transform.localToWorldMatrix.MultiplyPoint(p1);
                p2 = transform.localToWorldMatrix.MultiplyPoint(p2);
                Gizmos.DrawLine(new Vector3(p1.x, p1.y, 0), new Vector3(p2.x, p2.y, 0));
            }
        }
    }
}
