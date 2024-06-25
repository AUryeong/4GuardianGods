using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class Enemy : Unit
    {
        public float range;
        public Projectile enemyProjectile;

        private void Start()
        {
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 4, Attack);
        }

        private void Attack()
        {
            float x = GameManager.Instance.playerUnit.transform.position.x - transform.position.x;
            var obj = Instantiate(enemyProjectile);
            obj.velocity = new Vector2(x < 0 ? -1 : 1, 0);
            obj.transform.position = transform.position;
            obj.GetComponent<UnitMover>().velocity = new Vector2(x < 0 ? -1 : 1, 0);
        }

        public virtual void OnFixedUpdate()
        {
            float distance = Vector3.Distance(GameManager.Instance.playerUnit.transform.position, transform.position);
            if (distance < range)
            {
                unitAnimator.PlayAnimationClip(UnitAnimationType.Special);
            }

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