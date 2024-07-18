using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class CowEnemy : Enemy
    {
        [SerializeField] private UnitCollider unitCollider;

        public override bool IsAttacking => dashDuration > 0;

        private float dashDuration;
        private const float DASH_DURATION = 1;

        protected override void Start()
        {
            base.Start();
            unitCollider.SetColliderAction(ColliderAction);
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, -1, Attack);
        }


        public override void OnFixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            if (base.IsAttacking)
            {
                unitAnimator.UpdateAnimation(deltaTime);
                return;
            }
            if (!IsAttacking)
            {
                UpdateAttack(deltaTime);
                UpdateAnimState();
                UpdateDirection(deltaTime);
                SetFlip();
                unitAnimator.UpdateAnimation(deltaTime);
            }
            else
            {
                dashDuration -= Time.deltaTime;
                unitAnimator.SetAnimationState(UnitAnimationType.Walk);

                unitCollider.UpdateCollider();

                UpdateVelocity();

                unitMover.UpdateMove(deltaTime);
                unitAnimator.UpdateAnimation(deltaTime);
            }
        }

        protected override void Attack()
        {
            base.Attack();

            unitCollider.ClearColliders();
            CameraManager.Instance.Shake(0.2f, 2f, 3f);
            dashDuration = DASH_DURATION;
            SoundManager.Instance.PlaySoundSfx("Enemy_Cow", 1, 0.5f);
        }
    }
}