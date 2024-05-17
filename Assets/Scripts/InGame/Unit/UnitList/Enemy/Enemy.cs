using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class Enemy : Unit
    {
        private void Start()
        {
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);
        }

        public virtual void OnFixedUpdate()
        {
            float deltaTime = Time.deltaTime;
            UpdateVelocity();
            UpdateAnimState();
            unitAnimator.UpdateAnimation(deltaTime);
            unitMover.UpdateMove(deltaTime);
        }

        private void UpdateVelocity()
        {
            float x = GameManager.Instance.playerUnit.transform.position.x - transform.position.x;
            unitMover.velocity.x += Mathf.Sign(x);
        }
    }
}