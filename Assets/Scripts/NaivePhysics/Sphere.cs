using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaivePhysics
{
    public class Sphere : NaiveEngine.Shape
    {
        [SerializeField, Range(0.1f, 5.0f)]
        public float m_fRadius = 1.0f;

        #region Properties

        public override bool IsTrigger => false;

        #endregion

        protected override Rect CalculateBounds()
        {
            return new Rect(Position.x - m_fRadius,
                            Position.y - m_fRadius,
                            m_fRadius * 2.0f,
                            m_fRadius * 2.0f);
        }

        public override void DrawShape()
        {
            Gizmos.DrawWireSphere(Position, m_fRadius);
        }
    }
}