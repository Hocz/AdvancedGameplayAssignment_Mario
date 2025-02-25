using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaivePhysics
{
    public class Collision
    {
        public NaiveEngine.Shape    A;
        public NaiveEngine.Shape    B;
        public Vector2              m_vPosition;
        public Vector2              m_vNormal;
        public float                m_fPenetration;

        public bool Contains(NaiveEngine.Shape shape)
        {
            return shape == A || shape == B;
        }

        public NaiveEngine.Shape GetOther(NaiveEngine.Shape shape)
        {
            return shape == A ? B : 
                   shape == B ? A : null;
        }

        public Vector2 GetNormal(NaiveEngine.Shape shape)
        {
            return shape == A ? -m_vNormal : m_vNormal;
        }
    }
}