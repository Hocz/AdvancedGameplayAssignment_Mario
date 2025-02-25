using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaivePhysics
{
    public class Triangle : NaiveEngine.Shape
    {
        [SerializeField]
        public Vector2[]    m_vertices = new Vector2[] { new Vector2(-1.0f, -0.5f), new Vector2(0.2f, 1.2f), new Vector2(0.8f, -0.2f) };

        #region Properties

        public Vector2[] WorldPositions => System.Array.ConvertAll(m_vertices, v => (Vector2)transform.TransformPoint(v));

        public IEnumerable<Vector2[]> Lines
        {
            get
            {
                Vector2[] positions = WorldPositions;
                for (int i = 0; i < positions.Length; ++i)
                {
                    yield return new Vector2[] { positions[i], positions[(i + 1) % positions.Length] };
                }
            }
        }

        public IEnumerable<Plane> Planes
        {
            get
            {
                Vector2[] positions = WorldPositions;
                for (int i = 0; i < positions.Length; ++i)
                {
                    Vector3 vEdge = Vector3.Normalize(positions[(i + 1) % positions.Length] - positions[i]);
                    Vector3 vNormal = Vector3.Cross(Vector3.forward, vEdge);
                    vNormal *= Mathf.Sign(Vector3.Dot(positions[i] - positions[(i + 2) % positions.Length], vNormal));
                    yield return new Plane(vNormal, positions[i]);
                }
            }
        }

        public override bool IsTrigger => false;

        #endregion

        protected override Rect CalculateBounds()
        {
            Vector2 vMin = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 vMax = new Vector2(-float.MaxValue, -float.MaxValue);
            foreach (Vector2 v in WorldPositions)
            {
                vMin.x = Mathf.Min(vMin.x, v.x);
                vMin.y = Mathf.Min(vMin.y, v.y);
                vMax.x = Mathf.Max(vMax.x, v.x);
                vMax.y = Mathf.Max(vMax.y, v.y);
            }

            return new Rect(vMin, vMax - vMin);
        }

        public override void DrawShape()
        {
            foreach (Vector2[] line in Lines)
            {
                Gizmos.DrawLine(line[0], line[1]);
            }
        }
    }
}