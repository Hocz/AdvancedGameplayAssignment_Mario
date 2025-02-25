using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaivePhysics
{
    public class AlignedBox : NaiveEngine.Shape
    {
        [SerializeField]
        public Vector2      m_vSize = new Vector2(1.0f, 2.0f);

        private Vector2[]   m_corners;

        #region Properties

        public Vector2[] Corners => m_corners;

        public IEnumerable<Vector2[]> Lines
        {
            get
            {
                if (m_corners != null)
                {
                    for (int i = 0; i < m_corners.Length; ++i)
                    {
                        yield return new Vector2[] { m_corners[i], m_corners[(i + 1) % m_corners.Length] };
                    }
                }
            }
        }

        public override bool IsTrigger => false;

        #endregion

        protected override Rect CalculateBounds()
        {
            return new Rect(Position.x - m_vSize.x * 0.5f,
                            Position.y - m_vSize.y * 0.5f,
                            m_vSize.x, 
                            m_vSize.y);
        }

        protected override void OnMoved()
        {
            // calculate corners 
            m_corners = new Vector2[]
            {
                new Vector2(MinX, MinY), 
                new Vector2(MaxX, MinY), 
                new Vector2(MaxX, MaxY), 
                new Vector2(MinX, MaxY)
            };

            base.OnMoved();
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