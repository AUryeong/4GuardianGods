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
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 4, Attack);
            unitCollider.SetColliderAction(ColliderAction);
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
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

        private void ColliderAction(List<Collider2D> colliders)
        {
            foreach (var collider in colliders)
            {
                collider.GetComponent<UnitHit>().Hit(1);
            }
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