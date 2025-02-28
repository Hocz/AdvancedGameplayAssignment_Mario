using UnityEngine;
using Events;
using UnityEngine.SceneManagement;
using NaivePhysics;

namespace Game
{
    public class Mario : GameBody
    {
        [SerializeField]
        private float           m_fMoveVelocity = 4.0f;

        [SerializeField]
        private float           m_fJumpVelocity = 4.0f;

        private bool            m_bFaceRight;

        private float           m_fInvincible = -1.0f;

        private static Mario    sm_instance;


        #region Properties

        public static Mario Instance => sm_instance;

        #endregion

        private void OnEnable()
        {
            sm_instance = this;
        }

        public void Move()
        {
            // get player input
            float fDirection = 0.0f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                m_bFaceRight = true;
                fDirection = 1.0f;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                m_bFaceRight = false;
                fDirection = -1.0f;
            }

            // jump
            if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) && !_isJumping)
            {
                // is grounded?
                if (IsGrounded)
                {
                    _isJumping = true;
                    m_vVelocity.y = m_fJumpVelocity;
                }
            }

            // update mario velocity
            m_vVelocity.x = Mathf.MoveTowards(m_vVelocity.x, m_fMoveVelocity * fDirection, Time.deltaTime * m_fMoveVelocity * 4.0f);

            // update mario facing
            Quaternion qTarget = Quaternion.LookRotation(Vector3.left * (m_bFaceRight ? 1 : -1)) * Quaternion.Euler(0.0f, 90.0f, 0.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, qTarget, Time.deltaTime * 10.0f);
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

        public override void ResolveCollision(NaivePhysics.Collision collision, float fOtherMass, Vector2 vOtherVelocity)
        {
            base.ResolveCollision(collision, fOtherMass, vOtherVelocity);

            NaiveEngine.Shape other = collision.GetOther(Shape);
            Goal goal = other.GetComponent<Goal>();
            if (goal != null)
            {
                Debug.Log("Player Won!");
                GameManager.Instance._currentState = GameManager.GameState.GameWon;

                if (!GameManager.Instance._isGameOver)
                {
                    Playing.Instance.SetGameWon();
                }
            }
        }

        public void TakeDamage()
        {
            if (m_fInvincible < 0.0f)
            {
                m_iHP--;
                m_fInvincible = 1.0f;
            }

            if (m_iHP <=0)
            {
                m_iHP = 0;
                
                //game over
                GameManager.Instance._currentState = GameManager.GameState.GameOver;
                GameManager.Instance._isGameOver = true;
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