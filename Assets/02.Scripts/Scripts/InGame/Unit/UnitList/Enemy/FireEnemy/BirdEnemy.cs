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
            unitMover.velocity.x += direction == Direction.Left ? 1 : -1;
            unitMover.velocity.y += Mathf.Sign(GameManager.Instance.playerUnit.transform.position.y - transform.position.y);
        }
    }
}