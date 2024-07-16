using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class Projectile : PoolableUnit<Projectile>
    {
        [SerializeField] private ProjectileAnimator projectileAnimator;
        [SerializeField] private UnitCollider unitCollider;
        [SerializeField] private UnitMover unitMover;

        public float damage;
        public Vector2 velocity;

        private float duration;
        public float maxDuration = 10;

        private void Start()
        {
            unitMover.SetColliderAction(ColliderAction);
            projectileAnimator.PlayAnimationClip(ProjectileAnimationType.Start);
            unitCollider.SetColliderAction(ColliderAction);
        }

        private void ColliderAction(RaycastHit2D rayCast)
        {
            if (rayCast.collider.CompareTag("Brush"))
            {
                rayCast.collider.GetComponent<Drawing>().UnitHit.Hit(damage);
                SoundManager.Instance.PlaySoundSfx("Hit_Brush", 1.5f, 1);
                SoundManager.Instance.PlaySoundSfx("Hit_Brush2", 0.6f, 0.5f);
                SoundManager.Instance.PlaySoundSfx("Hit_Brush4", 1, 0.8f);
                CameraManager.Instance.Shake(0.3f, 2, 3f);
            }
            gameObject.SetActive(false);
        }

        private void ColliderAction(List<Collider2D> colliders)
        {
            foreach (var collider in colliders)
            {
                collider.GetComponent<UnitHit>().Hit(1);
            }
        }

        private void FixedUpdate()
        {
            unitMover.velocity = velocity;
            unitMover.UpdateMove(Time.fixedDeltaTime);
            unitCollider.UpdateCollider();
            projectileAnimator.SetAnimationState(ProjectileAnimationType.Loop);
            
            duration += Time.fixedDeltaTime;
            if (duration > maxDuration)
            {
                duration -= maxDuration;
                gameObject.SetActive(false);
            }
        }
    }
}