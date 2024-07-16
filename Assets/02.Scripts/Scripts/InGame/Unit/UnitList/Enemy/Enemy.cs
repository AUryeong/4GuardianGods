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
        [SerializeField] protected float attackCooltime;
        protected float attackDuration;

        [SerializeField] private GameObject enemyDieEffect;

        public virtual bool IsAttacking => unitAnimator.IsPlayAnimation(UnitAnimationType.Special);

        protected virtual void Start()
        {
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);
            unitHit.SetDieAction(Die);
        }

        public virtual void OnEnter()
        {
        }
        
        public virtual void OnExit()
        {
        }

        protected virtual void Attack()
        {
        }

        protected virtual void Die()
        {
            DrawManager.Instance.SetMaxBrush();
            gameObject.SetActive(false);
            if (enemyDieEffect != null)
            {
                Instantiate(enemyDieEffect, transform.position, Quaternion.identity);
            }
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
                SetFlip();
                unitAnimator.UpdateAnimation(deltaTime);
                unitMover.UpdateMove(deltaTime);
            }
            else
            {
                unitAnimator.UpdateAnimation(deltaTime);
            }
        }

        protected void SetFlip()
        {
            unitAnimator.IsFlip = direction == Direction.Left;
        }

        protected void UpdateAttack(float deltaTime)
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
                attackDuration += attackCooltime;
                SetFlip();
                unitAnimator.PlayAnimationClip(UnitAnimationType.Special);
            }
        }

        protected void UpdateDirection(float deltaTime)
        {
            directionDuration += deltaTime;
            if (directionDuration > DIRECTION_COOLTIME)
            {
                directionDuration -= DIRECTION_COOLTIME;
                float x = GameManager.Instance.playerUnit.transform.position.x - transform.position.x;
                direction = x > 0 ? Direction.Left : Direction.Right;
            }
        }

        protected virtual void UpdateVelocity()
        {
            unitMover.velocity.x += direction == Direction.Left ? 1 : -1;
        }
    }
}