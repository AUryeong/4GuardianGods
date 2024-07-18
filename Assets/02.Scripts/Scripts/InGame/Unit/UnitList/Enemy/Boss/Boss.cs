using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame.Unit
{
    public class Boss : Unit
    {
        [SerializeField] private Transform[] bossTransforms;

        private float attackDuration;
        private const float ATTACK_DURATION = 5;

        public bool IsControllbale { get; set; }
        private Coroutine attackCoroutine;

        private bool isAttacking;
        private bool isMoving;
        [SerializeField] private UnitCollider moveCollider;

        private const int PATTERN_COUNT = 1;

        private void Awake()
        {
            unitHit.SetHitAction(HitAction);
            unitMover.SetColliderAction(ColliderAction);
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);

            moveCollider.SetColliderAction(MoveColliderAction);
        }

        protected void MoveColliderAction(List<Collider2D> colliders)
        {
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                    collider.GetComponent<UnitHit>().Hit(1);
            }
        }

        private void HitAction()
        {
            UnitAnimator.Hit();
            Instantiate(GameManager.Instance.dieEffect, transform.position, Quaternion.identity);
            CameraManager.Instance.Shake(0.3f, 3f, 8f);
        }

        private void ColliderAction(RaycastHit2D rayCast)
        {
            if (rayCast.collider.CompareTag("Brush"))
            {
                var drawing = rayCast.collider.GetComponent<Drawing>();
                if (drawing.Type == DrawingType.Remove)
                    return;

                if (isMoving)
                {
                    CameraManager.Instance.Shake(0.2f, 2f, 4f);
                    drawing.UnitHit.Hit(1000);
                }
                else
                {
                    CameraManager.Instance.Shake(0.5f, 5f, 12f);
                    drawing.UnitHit.Hit(1000);
                    CancelAttack();
                }
            }
        }

        public void StartAttack()
        {
            Attack(0);
        }

        private void CancelAttack()
        {
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            isMoving = false;
            isAttacking = false;

            UnitMover.IsPassPlatform = false;

            unitMover.velocity = Vector2.zero;

            unitAnimator.SpriteRenderer.transform.rotation = Quaternion.identity;
            unitAnimator.IsFlip = GameManager.Instance.playerUnit.transform.position.x - transform.position.x > 0;

            unitAnimator.SetAnimationState(UnitAnimationType.Idle);

            moveCollider.gameObject.SetActive(false);
        }

        public void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            if (IsControllbale)
            {
                UpdateAttack(deltaTime);
            }
            unitAnimator.UpdateAnimation(deltaTime);
        }

        private void UpdateAttack(float deltaTime)
        {
            if (isAttacking)
                return;

            attackDuration += deltaTime;
            if (attackDuration > ATTACK_DURATION)
            {
                attackDuration -= ATTACK_DURATION;
                Attack(Random.Range(0, PATTERN_COUNT));
            }
        }

        private void Attack(int pattern)
        {
            isAttacking = true;
            switch (pattern)
            {
                case 0:
                    attackCoroutine = StartCoroutine(PatternMove());
                    break;
            }
        }

        private IEnumerator PatternMove()
        {
            isMoving = true;

            moveCollider.gameObject.SetActive(true);
            moveCollider.ClearColliders();

            bossTransforms.Shuffle();
            unitAnimator.SetAnimationState(UnitAnimationType.Special2);
            for (int i = 0; i < bossTransforms.Length; i++)
            {
                CameraManager.Instance.Shake(0.2f, 3f, 2f);
                yield return Move(bossTransforms[i].position);
            }

            CancelAttack();
        }

        private IEnumerator Move(Vector3 position)
        {
            var velocity = (position - transform.position).normalized;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

            unitAnimator.IsFlip = false;
            unitAnimator.SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle + 180);

            while (true)
            {
                float deltaTime = Time.deltaTime;
                if (Vector3.Distance(position, transform.position) < deltaTime * unitMover.speed)
                    break;

                unitMover.velocity = velocity;
                unitMover.UpdateMove(deltaTime);

                moveCollider.UpdateCollider();

                yield return null;
            }
        }
    }
}