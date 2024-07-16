using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public enum Direction
    {
        Left,
        Right,
    }
    public class Enemy : Unit
    {
        protected Direction direction;
        private float directionDuration;
        private const float DIRECTION_COOLTIME = 0.2f;

        public float attackRange;
        [SerializeField] private float attackCooltime;
        private float attackDuration;

        public virtual bool IsAttacking => unitAnimator.IsPlayAnimation(UnitAnimationType.Special);

        protected virtual void Start()
        {
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);
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
            float deltaTime = Time.fixedDeltaTime;
            if (!IsAttacking)
            {
                UpdateAttack(deltaTime);
                UpdateDirection(deltaTime);
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

        private void UpdateAttack(float deltaTime)
        {
            if (IsAttacking)
                return;

            if (attackDuration > 0)
            {
                attackDuration -= deltaTime;
                return;
            }

            float distance = Vector3.Distance(GameManager.Instance.playerUnit.transform.position, transform.position);
            if (distance < attackRange)
            {
                SetFlip();
                attackDuration = attackCooltime;
                unitAnimator.PlayAnimationClip(UnitAnimationType.Special);
            }
        }

        private void UpdateDirection(float deltaTime)
        {
            directionDuration += deltaTime;
            if (directionDuration > DIRECTION_COOLTIME)
            {
                directionDuration -= DIRECTION_COOLTIME;
                float x = GameManager.Instance.playerUnit.transform.position.x - transform.position.x;
                direction = x > 0 ? Direction.Left : Direction.Right;
            }
        }


        private void UpdateVelocity()
        {
            unitMover.velocity.x += (direction == Direction.Left) ? 1 : -1;
        }
    }
}