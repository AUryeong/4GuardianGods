using UnityEngine;

namespace InGame.Unit
{
    public class BossUnitAnimator : UnitAnimator
    {
        [SerializeField] private ProjectileAnimator projectileAnimator;

        public override void UpdateAnimation(float deltaTime)
        {
            base.UpdateAnimation(deltaTime);
            projectileAnimator.UpdateAnimation(deltaTime);
        }

        public override void Hit()
        {
            base.Hit();
            projectileAnimator.Hit();
        }
    }
}