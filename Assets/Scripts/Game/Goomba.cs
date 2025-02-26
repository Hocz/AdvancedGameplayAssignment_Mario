using Events;
using NaivePhysics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class Goomba : GameBody
    {

        [SerializeField] 
        private float       m_fMoveVelocity = 2.0f;

        private bool        m_bWalkingRight;


        public bool _attack = false;
        public bool _block = false;
        public bool _heal = false;

        private float _arcTime = 0;
        private float _arcDuration = 2;


        private void OnEnable()
        {
            m_bWalkingRight = Random.value > 0.5f;
        }

        public override void TickBody()
        {
            if (GameManager.Instance._currentState == GameManager.GameState.Playing)
            {
                Move();     
            }
            if (GameManager.Instance._currentState == GameManager.GameState.Combat)
            {
                if (_attack)
                {
                    MoveAttack();
                }
            }
            base.TickBody();
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

        public void MoveAttack()
        {
            // jump in arc
            if (_arcTime < _arcDuration)
            {
                _arcTime += Time.fixedDeltaTime;

                float t = _arcTime / _arcDuration;

                Vector3 pos = Vector3.Lerp(
                    Combat.Instance.GoombaSpawnPos.position,
                    Mario.Instance.AttackTarget.position,
                    t);

                float arc = Mathf.Sin(t * Mathf.PI) * 2;

                transform.position = new Vector3(pos.x, pos.y + arc, pos.z);
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
                    if (GameManager.Instance._currentState != GameManager.GameState.Combat)
                    {
                        Combat.Instance._playerTurn = true;
                        Combat.Instance._goomba = this;

                        EventHandler.Main.PushEvent(Combat.Instance);
                    }    
                    
                    //// do death logic
                    //mario.StartCoroutine(DeathLogic(transform));
                }
                else
                {

                    if (GameManager.Instance._currentState != GameManager.GameState.Combat)
                    {
                        Combat.Instance._playerTurn = false;
                        Combat.Instance._goomba = this;

                        EventHandler.Main.PushEvent(Combat.Instance);
                    }
                    else if (GameManager.Instance._currentState == GameManager.GameState.Combat)
                    {
                        Debug.Log("Hit!");
                        _attack = false;
                        mario.TakeDamage();
                        Combat.Instance.InitializeCombat();
                    }
                }
            }

            base.ResolveCollision(collision, fOtherMass, vOtherVelocity);
        }

        public void TakeDamage()
        {
            m_iHP--;
            if (m_iHP <= 0)
            {
                m_iHP = 0;
                Destroy(Shape);
                Destroy(this);
                Destroy(gameObject);
            }

            //if (m_fInvincible < 0.0f)
            //{
            //    m_iHP--;
            //    m_fInvincible = 1.0f;
            //    UpdateHealth();
            //    StartCoroutine(InvincibleRendering());
            //}
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