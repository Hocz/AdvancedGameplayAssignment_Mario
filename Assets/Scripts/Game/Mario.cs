using System.Collections;
using UnityEngine;

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
            if (Input.GetKey(KeyCode.D))
            {
                m_bFaceRight = true;
                fDirection = 1.0f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                m_bFaceRight = false;
                fDirection = -1.0f;
            }

            // jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // is grounded?
                if (IsGrounded)
                {
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
            if (GameManager.Instance._currentState == GameManager.GameState.Playing)
            {
                Move();
                
                m_fInvincible -= Time.fixedDeltaTime;
            }
            base.TickBody();

        }

        public void TakeDamage()
        {
            if (m_iHP <=0)
            {
                m_iHP = 0;
                //game over
            }
            
            //if (m_fInvincible < 0.0f)
            //{
            //    m_iHP--;
            //    m_fInvincible = 1.0f;
            //    UpdateHealth();
            //    StartCoroutine(InvincibleRendering());
            //}
        }


        IEnumerator InvincibleRendering()
        {
            // flash renderers
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            while (m_fInvincible > 0.0f)
            {
                foreach (MeshRenderer mr in renderers)
                {
                    mr.enabled = !mr.enabled;
                }
                yield return new WaitForSeconds(0.33f);
            }

            // restore renderers
            foreach (MeshRenderer mr in renderers)
            {
                mr.enabled = true;
            }
        }
    }
}