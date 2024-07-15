using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class Enemy : Unit
    {
        public float range;

        protected virtual void Start()
        {
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 0, SetFlip);
            unitHit.SetDieAction(Die);
        }

        protected virtual void Attack()
        {
        }

        protected virtual void Die()
        {
            DrawManager.Instance.SetMaxBrush();
        }

        public virtual void OnFixedUpdate()
        {
            float deltaTime = Time.deltaTime;
            CheckAttack();

            if (!unitAnimator.IsPlayAnimation(UnitAnimationType.Special))
            {
                UpdateVelocity();
                UpdateAnimState();
                unitAnimator.UpdateAnimation(deltaTime);
                unitMover.UpdateMove(deltaTime);
            }
            else
            {
                unitAnimator.UpdateAnimation(deltaTime);
            }
        }

        private void SetFlip()
        {
            unitAnimator.IsFlip = GameManager.Instance.playerUnit.transform.position.x - transform.position.x > 0;
        }

        private void CheckAttack()
        {
            float distance = Vector3.Distance(GameManager.Instance.playerUnit.transform.position, transform.position);
            if (distance < range)
                unitAnimator.PlayAnimationClip(UnitAnimationType.Special);
        }

        private void UpdateVelocity()
        {
            float x = GameManager.Instance.playerUnit.transform.position.x - transform.position.x;
            unitMover.velocity.x += Mathf.Sign(x);
        }
    }
}