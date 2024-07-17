using UnityEngine;

namespace InGame.Unit
{
    public class BirdEnemy : ProjectileEnemy
    {
        protected override void Start()
        {
            base.Start();
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 3, Attack);
        }
        
        protected override void UpdateVelocity()
        {
            float distance = Vector3.Distance(GameManager.Instance.playerUnit.transform.position, transform.position);
            var vector = new Vector2(direction == Direction.Left ? 1 : -1, Mathf.Sign(GameManager.Instance.playerUnit.transform.position.y - transform.position.y));
            if (distance < attackRange)
            {
                unitMover.velocity -= vector;
            }
            else
            {
                unitMover.velocity += vector;
            }
        }
    }
}