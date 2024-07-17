using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class DogEnemy : Enemy
    {
        [SerializeField] private UnitCollider unitCollider;

        protected override void Start()
        {
            base.Start();
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 5, Attack);
            unitCollider.SetColliderAction(ColliderAction);
        }

        public override void OnFixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            if (!IsAttacking)
            {
                UpdateAttack(deltaTime);
            }
            UpdateDirection(deltaTime);
            UpdateVelocity();
            UpdateAnimState();
            SetFlip();
            unitAnimator.UpdateAnimation(deltaTime);
            unitMover.UpdateMove(deltaTime);
        }

        protected override void Attack()
        {
            base.Attack();
            
            unitCollider.ClearColliders();
            unitCollider.UpdateCollider();
            
            CameraManager.Instance.Shake(0.2f, 2f,3f);
        }
    }
}