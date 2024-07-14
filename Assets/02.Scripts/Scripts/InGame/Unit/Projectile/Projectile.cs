using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class Projectile : PoolableUnit<Projectile>
    {
        [SerializeField] private ProjectileAnimator projectileAnimator;
        [FormerlySerializedAs("projectileCollider")]
        [SerializeField] private UnitCollider unitCollider;
        [SerializeField] private UnitMover unitMover;

        private void Start()
        {
            projectileAnimator?.PlayAnimationClip(ProjectileAnimationType.Loop);
        }

        private void FixedUpdate()
        {
            if (unitMover.velocity.x != 0 && projectileAnimator != null)
                projectileAnimator.IsFlip = unitMover.velocity.x > 0;
            unitMover?.UpdateMove(Time.fixedDeltaTime);
        }
    }
}