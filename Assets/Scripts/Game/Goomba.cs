using NaivePhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Goomba : GameBody
    {
        [SerializeField]
        public float        m_fMoveVelocity = 2.0f;

        private Animator    m_animator;
        private bool        m_bWalkingRight;

        private void OnEnable()
        {
            m_animator = GetComponent<Animator>();
            m_bWalkingRight = Random.value > 0.5f;
        }

        private void Update()
        {
            m_animator.SetFloat("Velocity", Mathf.Clamp(m_vVelocity.x / m_fMoveVelocity, -1.0f, 1.0f));
        }

        public override void TickBody()
        {
            m_vVelocity.x = Mathf.MoveTowards(m_vVelocity.x,
                                              m_fMoveVelocity * (m_bWalkingRight ? 1.0f : -1.0f),
                                              Time.fixedDeltaTime * m_fMoveVelocity);

            base.TickBody();
        }

        public override void ResolveCollision(NaivePhysics.Collision collision, float fOtherMass, Vector2 vOtherVelocity)
        {
            // did we collide with a wall?
            Vector2 vCollisionNormal = collision.GetNormal(Shape);
            float fDot = Vector2.Dot(vCollisionNormal, Vector2.right);
            if (Mathf.Abs(fDot) > 0.75f)
            {
                if ((fDot < 0.0f && m_bWalkingRight) ||
                    (fDot > 0.0f && !m_bWalkingRight))
                {
                    m_bWalkingRight = !m_bWalkingRight;
                }
            }

            // is mario?
            NaiveEngine.Shape other = collision.GetOther(Shape);
            Mario mario = other.GetComponent<Mario>();
            if (mario != null)
            {
                fDot = Vector2.Dot(vCollisionNormal, Vector2.up);
                if (fDot < -0.7f)
                {
                    // do death logic
                    mario.StartCoroutine(DeathLogic(transform));
                }
                else
                {
                    mario.TakeDamage();
                }
            }

            base.ResolveCollision(collision, fOtherMass, vOtherVelocity);
        }

        IEnumerator DeathLogic(Transform goombaTransform)
        {
            Destroy(Shape);
            Destroy(this);

            for (float f = 0.0f; f < 1.0f; f += Time.deltaTime)
            {
                goombaTransform.localScale = new Vector3(1.0f, 1.0f - f, 1.0f);
                yield return null;
            }

            Destroy(goombaTransform.gameObject);
        }
    }
}