using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class ArmEnemy : Enemy
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
            base.OnFixedUpdate();
        }

        private void ColliderAction(List<Collider2D> colliders)
        {
            foreach (var collider in colliders)
            {
                collider.GetComponent<UnitHit>().Hit(1);
            }
        }

        protected override void UpdateVelocity()
        {
            unitMover.velocity.x += direction == Direction.Left ? 1 : -1;
            unitMover.velocity.y += Mathf.Sign(GameManager.Instance.playerUnit.transform.position.y - transform.position.y);
        }

        protected override void Attack()
        {
            base.Attack();

            unitCollider.ClearColliders();
            unitCollider.UpdateCollider();

            CameraManager.Instance.Shake(0.2f, 2f, 3f);
        }
    }
}