using NaivePhysics;
using UnityEngine;

namespace Game
{
    public class GameBody : NaivePhysics.NaiveBody
    {
        [SerializeField]
        public float    m_fStepHeight = 0.07f;

        private bool    m_bIsGrounded;

        #region Properties

        public bool IsGrounded => m_bIsGrounded;

        #endregion

        public override void TickBody()
        {
            base.TickBody();
            m_bIsGrounded = false;
        }

        public override void ResolveCollision(NaivePhysics.Collision collision, float fOtherMass, Vector2 vOtherVelocity)
        {
            // did we collide with floor?
            Vector2 vCollisionNormal = collision.GetNormal(Shape);
            float fDot = Vector2.Dot(vCollisionNormal, Vector2.up);
            if (fDot > 0.75f)
            {
                m_bIsGrounded = true;
            }

            // should we do a step?
            NaiveEngine.Shape other = collision.A == Shape ? collision.B : collision.A;
            if (Mathf.Abs(Vector2.Dot(vCollisionNormal, Vector2.right)) > 0.9f &&
                other.MaxY < Shape.MinY + m_fStepHeight)
            {
                transform.position += Vector3.up * (other.MaxY - Shape.MinY);
                return;
            }

            base.ResolveCollision(collision, fOtherMass, vOtherVelocity);
        }
    }
}