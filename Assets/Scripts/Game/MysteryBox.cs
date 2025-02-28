using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaivePhysics;

namespace Game
{
    public class MysteryBox : Platform
    {
        private bool m_bHasCoin = true;

        [SerializeField] private GameObject _coin;

        #region Properties

        public override bool IsTrigger => m_bHasCoin;

        #endregion

        public override void OnEnterCollision(NaivePhysics.Collision collision, NaiveEngine.Shape other)
        {
            base.OnEnterCollision(collision, other);

            Vector2 vCollisionNormal = collision.GetNormal(this);
            Mario mario = other.GetComponent<Mario>();

            if (mario != null &&
                mario.Shape.MaxY < Position.y &&
                Vector2.Dot(vCollisionNormal, Vector2.up) > 0.7f)
            {

                StartCoroutine(MoveCoin());

                m_bHasCoin = false;
            }
        }

        IEnumerator MoveCoin()
        {
            for (float t = 0.0f; t < 0.75f; t += Time.deltaTime)
            {
                _coin.transform.localPosition = new Vector3(0, 0 + t, 0);
                yield return null;
            }
            Destroy(_coin);
        }

    }
}