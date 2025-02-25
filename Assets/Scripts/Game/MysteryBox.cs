using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaivePhysics;

namespace Game
{
    public class MysteryBox : Platform
    {
        private bool m_bHasCoin = true;

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
                GetComponent<Animator>().SetTrigger("Coin");
                m_bHasCoin = false;
            }
        }
    }
}