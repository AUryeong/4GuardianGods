using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class Projectile : PoolableUnit<Projectile>
    {
        [SerializeField] private ProjectileAnimator projectileAnimator;
        [SerializeField] private UnitMover unitMover;

        public Vector2 velocity;

        public float damage;

        private void Start()
        {
            projectileAnimator?.PlayAnimationClip(ProjectileAnimationType.Loop);
        }

        private void FixedUpdate()
        {
            if (unitMover == null)
                return;

            if (unitMover.velocity.x != 0 && projectileAnimator != null)
                projectileAnimator.IsFlip = unitMover.velocity.x > 0;
            unitMover.velocity = velocity;
            unitMover?.UpdateMove(Time.fixedDeltaTime);
        }
    }
}