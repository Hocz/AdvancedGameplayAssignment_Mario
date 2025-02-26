using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        [SerializeField]
        public Vector2      m_vOffset = Vector2.zero;

        private Vector2     m_vPrediction = Vector2.zero;

        void Update()
        {
            if (GameManager.Instance._currentState != GameManager.GameState.Combat)
            {
                // move camera to focus on Mario
                if (Mario.Instance != null)
                {
                    // predict mario's future position
                    Vector2 vVelocity = Mario.Instance.Velocity;
                    vVelocity.y = 0.0f; // Mathf.Clamp(vVelocity.y, -10.0f, 0.0f);
                    m_vPrediction = Vector2.MoveTowards(m_vPrediction, vVelocity, Time.deltaTime * 3.0f);

                    // move camera towards target
                    Vector3 vTarget = Mario.Instance.transform.position + (Vector3)m_vOffset + (Vector3)m_vPrediction;
                    vTarget.y = transform.position.y;
                    vTarget.z = transform.position.z;
                    transform.position += (vTarget - transform.position) * Time.deltaTime * 10.0f;
                }
            }
            else
            {
                transform.position = Combat.Instance.CameraPos.position;
            }
        }
    }
}