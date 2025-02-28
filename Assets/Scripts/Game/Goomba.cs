using Events;
using NaivePhysics;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class Goomba : GameBody
    {

        [SerializeField] 
        private float       m_fMoveVelocity = 2.0f;

        private bool        m_bWalkingRight;

        private float       m_fInvincible = -1.0f;


        private void OnEnable()
        {
            m_bWalkingRight = Random.value > 0.5f;
            m_iMaxHP = 2;
            m_iHP = m_iMaxHP;
        }

        public override void TickBody()
        {
            if (GameManager.Instance._currentState != GameManager.GameState.Paused)
            {
                if (GameManager.Instance._currentState == GameManager.GameState.Playing)
                {
                    Move();
                }
                m_fInvincible -= Time.fixedDeltaTime;

                base.TickBody();
            }
        }

        public void Move()
        {
            // move towards player
            if (Vector3.Distance(Mario.Instance.transform.position, transform.position) <= 3)
            {
                Debug.Log("Move To Player!");
                m_vVelocity.x = Mathf.MoveTowards(m_vVelocity.x,
                                              (Mario.Instance.transform.position.x - transform.position.x) * m_fMoveVelocity,
                                              Time.fixedDeltaTime * m_fMoveVelocity);
            }
            // move random direction
            else
            {
                Debug.Log("Move Freely!");
                m_vVelocity.x = Mathf.MoveTowards(m_vVelocity.x,
                                              m_fMoveVelocity * (m_bWalkingRight ? 1.0f : -1.0f),
                                              Time.fixedDeltaTime * m_fMoveVelocity);
            }
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
                    if (GameManager.Instance._currentState == GameManager.GameState.Playing)
                    {
                        Combat.Instance._playerTurn = true;
                        Combat.Instance._goomba = this;

                        Playing.Instance._marioLastPosition = Mario.Instance.transform.position;
                        Playing.Instance._goombaLastPosition = transform.position;

                        Debug.Log("Enter Combat!");

                        GameManager.Instance._currentState = GameManager.GameState.Combat;
                        EventHandler.Main.PushEvent(Combat.Instance);
                    }
                }
                else
                {

                    if (GameManager.Instance._currentState == GameManager.GameState.Playing)
                    {
                        Combat.Instance._playerTurn = false;
                        Combat.Instance._goomba = this;

                        Playing.Instance._marioLastPosition = Mario.Instance.transform.position;
                        Playing.Instance._goombaLastPosition = transform.position;

                        Debug.Log("Enter Combat!");

                        GameManager.Instance._currentState = GameManager.GameState.Combat;
                        EventHandler.Main.PushEvent(Combat.Instance);
                    }
                }
            }

            base.ResolveCollision(collision, fOtherMass, vOtherVelocity);
        }

        public void TakeDamage()
        {
            if (m_fInvincible < 0.0f)
            {
                m_iHP--;
                m_fInvincible = 1.0f;
            }

            if (m_iHP <= 0)
            {
                m_iHP = 0;
                Destroy(Shape);
                Destroy(this);
                Destroy(gameObject);
            }


        }
        public void Heal()
        {
            if (m_iHP <= m_iMaxHP && m_iHP != 0)
            {
                m_iHP++;
            }
        }
    }
}