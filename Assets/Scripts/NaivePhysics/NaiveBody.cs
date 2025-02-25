using UnityEngine;

namespace NaivePhysics
{
    [RequireComponent(typeof(NaiveEngine.Shape))]
    public class NaiveBody : MonoBehaviour
    {
        [SerializeField]
        public float                m_fMass = 1.0f;

        protected NaiveEngine.Shape m_shape;
        protected Vector2           m_vVelocity;

        const float                 BOUNCE = 0.1f;
        const float                 FRICTION = 0.1f;
        const float                 POSITION_CORRECTION_PRC = 0.8f;
        const float                 POSITION_CORRECTION_SLOP = 0.01f;

        #region Properties

        public NaiveEngine.Shape Shape => m_shape;

        public Vector2 Velocity => m_vVelocity;

        #endregion

        protected virtual void Start()
        {
            m_shape = GetComponent<NaiveEngine.Shape>();
        }

        public virtual void TickBody()
        {
            // add gravity force
            m_vVelocity += NaiveEngine.GRAVITY * Vector2.down * Time.fixedDeltaTime;

            // apply friction
            m_vVelocity *= (1.0f - FRICTION * Time.fixedDeltaTime);

            // cap max velocity
            if (m_vVelocity.magnitude > NaiveEngine.MAX_VELOCITY)
            {
                m_vVelocity = m_vVelocity.normalized * NaiveEngine.MAX_VELOCITY;
            }

            // update position
            transform.position += (Vector3)m_vVelocity * Time.fixedDeltaTime;
        }

        public virtual void ResolveCollision(Collision collision, float fOtherMass, Vector2 vOtherVelocity)
        {
            NaiveBody bodyA = collision.A.GetComponent<NaiveBody>();
            NaiveBody bodyB = collision.B.GetComponent<NaiveBody>();
            if (bodyA == null &&
                bodyB == null)
            {
                return;
            }

            // calculate relative velocity
            Vector2 vRelativeVelocity = vOtherVelocity - m_vVelocity;
            Vector2 vCollisionNormal = collision.GetNormal(Shape);
            float fVelocityAlongNormal = Vector2.Dot(vRelativeVelocity, vCollisionNormal);

            // are velocities separating the bodies?
            if (fVelocityAlongNormal <= 0.0f)
            {
                return;
            }

            // calculate impulse scalar
            float fMassInv = 1.0f / m_fMass;
            float fOtherMassInv = fOtherMass <= 0.001f ? 0.0f : (1.0f / fOtherMass);
            float fImpulse = -(1 + BOUNCE) * fVelocityAlongNormal;
            fImpulse /= fMassInv + fOtherMassInv;

            // apply impulse
            m_vVelocity -= fImpulse * vCollisionNormal * fMassInv;

            // positional correction
            Vector2 vCorrection = (Mathf.Max(collision.m_fPenetration - POSITION_CORRECTION_SLOP, 0.0f) / (fMassInv + fOtherMassInv)) * POSITION_CORRECTION_PRC * vCollisionNormal;
            transform.position += fMassInv * (Vector3)vCorrection;
        }
    }
}