using System.Collections;
using UnityEngine;

namespace Game
{
    public class Mario : GameBody
    {
        [SerializeField]
        public float            m_fMoveVelocity = 4.0f;

        [SerializeField]
        public float            m_fJumpVelocity = 8.0f;

        private Animator        m_animator;
        private bool            m_bFaceRight;
        private int             m_iHP = 2;
        private float           m_fInvincible = -1.0f;

        private static Mario    sm_instance;

        #region Properties

        public static Mario Instance => sm_instance;

        #endregion

        private void OnEnable()
        {
            m_animator = GetComponent<Animator>();
            sm_instance = this;
        }

        private void Update()
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
            Quaternion qTarget = Quaternion.LookRotation(Vector3.right * (m_bFaceRight ? 1 : -1)) * Quaternion.Euler(0.0f, 90.0f, 0.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, qTarget, Time.deltaTime * 6.0f);

            // update mario animations
            m_animator.SetFloat("Speed", Mathf.Abs(m_vVelocity.x) / m_fMoveVelocity);
            m_animator.SetBool("Jumping", !IsGrounded);
        }

        public override void TickBody()
        {
            m_fInvincible -= Time.fixedDeltaTime;
            base.TickBody();
        }

        public void TakeDamage()
        {
            if (m_fInvincible < 0.0f)
            {
                m_iHP--;
                m_fInvincible = 1.0f;
                UpdateHealth();
                StartCoroutine(InvincibleRendering());
            }
        }

        protected void UpdateHealth()
        {
            Transform body = transform.Find("Mario_Body");

            switch (m_iHP)
            {
                case 2:
                    {
                        // big mario
                        body.localScale = Vector3.one;
                        body.localPosition = Vector3.zero;
                        break;
                    }

                case 1:
                    {
                        // small mario
                        body.localScale = Vector3.one * 0.61f;
                        body.localPosition = new Vector3(0.0f, -0.138f, 0.0f);
                        break;
                    }
            }
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