using System;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame.Unit
{
    public enum BossAttackType
    {
        Move,
        Summon,
        EarthBreak,
        FireBall,
        Laser,
        Max,
    }

    public class Boss : Unit
    {
        private float attackDuration;

        public bool IsControllable { get; set; }
        private Coroutine attackCoroutine;

        private bool IsAttacking => IsUsing.Values.Any(x => x);
        private Dictionary<BossAttackType, bool> IsUsing = new((int)BossAttackType.Max);
        private BossAttackType prevType;

        [SerializeField] private ProjectileAnimator wingAnimator;

        [Header(nameof(BossAttackType.Move))]
        [SerializeField] private Transform[] bossTransforms;
        [SerializeField] private UnitCollider moveCollider;

        [Header(nameof(BossAttackType.Summon))]
        [SerializeField] private Dictionary<EnemyType, Transform> bossMobTransforms;

        [Header(nameof(BossAttackType.EarthBreak))]
        [SerializeField] private WarningObject earthBreakWarning;
        [SerializeField] private Projectile earthBreakProjectile;
        private Vector3 lastEarthBreakPlayerPosition;

        [Header(nameof(BossAttackType.FireBall))]
        [SerializeField] private Transform[] fireballTransform;
        [SerializeField] private Projectile fireballProjectile;
        [SerializeField] private WarningObject fireballWarning;

        [Header(nameof(BossAttackType.Laser))]
        [SerializeField] private WarningObject[] laserWarning;
        [SerializeField] private Projectile laserProjectile;


        private void Awake()
        {
            unitHit.SetHitAction(HitAction);
            unitHit.SetDieAction(DieAction);
            unitMover.SetColliderAction(ColliderAction);

            unitAnimator.SetAnimationState(UnitAnimationType.Idle);
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 9, SummonSkillAction);
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special4, 8, EarthBreakSkillAction);
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special5, -1, () => UIManager.Instance.BossDie());

            moveCollider.SetColliderAction(CheckColliderAction);
            moveCollider.gameObject.SetActive(false);

            wingAnimator.gameObject.SetActive(false);
            wingAnimator.SetAnimationState(ProjectileAnimationType.Start);
            wingAnimator.SetAnimationCallBack(ProjectileAnimationType.End, -1, () => wingAnimator.gameObject.SetActive(false));

            for (var type = BossAttackType.Move; type < BossAttackType.Max; type++)
                IsUsing.TryAdd(type, false);
        }

        private void DieAction()
        {
            if (!IsControllable)
                return;

            CancelAttack();
            IsControllable = false;

            SoundManager.Instance.PlaySoundSfx("Enemy_Growl", 1, 1.3f);

            CameraManager.Instance.Shake(2, 4f, 15f);

            unitAnimator.ClearPlayState();
            unitAnimator.PlayAnimationClip(UnitAnimationType.Special5);

            wingAnimator.ClearPlayState();
            wingAnimator.PlayAnimationClip(ProjectileAnimationType.End);
        }

        private void SetAnimatorFlip()
        {
            unitAnimator.SpriteRenderer.transform.rotation = Quaternion.identity;
            unitAnimator.IsFlip = GameManager.Instance.playerUnit.transform.position.x - transform.position.x > 0;
            wingAnimator.IsFlip = unitAnimator.IsFlip;
        }

        private void EarthBreakSkillAction()
        {
            if (IsUsing[BossAttackType.EarthBreak])
            {
                SoundManager.Instance.StopSoundAmbient("Enemy_Warning");

                var obj = Instantiate(earthBreakProjectile);
                obj.transform.position = lastEarthBreakPlayerPosition;

                SoundManager.Instance.PlaySoundSfx("Enemy_Dog");
                SoundManager.Instance.PlaySoundSfx("Enemy_Dog2", 1.2f, 1.2f);
                SoundManager.Instance.PlaySoundSfx("Enemy_Flower", 0.7f, 1.2f);
                CameraManager.Instance.Shake(0.4f, 4f, 12f);
                return;
            }
        }

        private void SummonSkillAction()
        {
            if (!IsControllable)
            {
                SoundManager.Instance.PlaySoundSfx("Enemy_Growl", 1, 1.3f);
                SoundManager.Instance.PlaySoundSfx("Enemy_Flower", 0.7f, 1.2f);

                wingAnimator.gameObject.SetActive(true);
                wingAnimator.PlayAnimationClip(ProjectileAnimationType.Start);
                wingAnimator.SetAnimationState(ProjectileAnimationType.Loop);
                return;
            }

            if (IsUsing[BossAttackType.Summon])
            {
                SoundManager.Instance.PlaySoundSfx("Enemy_Growl", 1, 1.3f);
                SoundManager.Instance.PlaySoundSfx("Enemy_Human", 1, 0.5f);
                SoundManager.Instance.PlaySoundSfx("Enemy_Flower", 0.7f, 1.2f);

                CameraManager.Instance.Shake(1, 3f, 6f);
                var enemyType = (EnemyType)Random.Range(0, (int)EnemyType.Max);
                var enemyParent = bossMobTransforms[enemyType];

                foreach (Transform pos in enemyParent)
                    GameManager.Instance.bossMap.CreateEnemy(enemyType, pos.position);
                return;
            }
        }

        private void CheckColliderAction(List<Collider2D> colliders)
        {
            foreach (var unitCollider in colliders.Where(unitCollider => unitCollider.CompareTag("Player")))
            {
                unitCollider.GetComponent<UnitHit>().Hit(1);
            }
        }

        private IEnumerator PatternSummon()
        {
            IsUsing[BossAttackType.Summon] = true;

            unitAnimator.PlayAnimationClip(UnitAnimationType.Special);
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);

            yield return new WaitForSeconds(5);

            CancelAttack();
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

                if (IsUsing[BossAttackType.Move])
                {
                    CameraManager.Instance.Shake(0.5f, 5f, 12f);
                    drawing.UnitHit.Hit(1000);
                    unitHit.Hit(1);
                    CancelAttack();
                }
                else
                {
                    CameraManager.Instance.Shake(0.5f, 5f, 12f);
                    drawing.UnitHit.Hit(1000);
                    CancelAttack();
                }
            }
        }

        public void OnEnter()
        {
            UnitAnimator.PlayAnimationClip(UnitAnimationType.Special);
        }

        public void StartAttack()
        {
            Attack(BossAttackType.Move);
        }

        private void CancelAttack()
        {
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            var keys = IsUsing.Keys.ToArray();
            foreach (var key in keys)
                IsUsing[key] = false;

            UnitMover.IsPassPlatform = false;

            unitMover.velocity = Vector2.zero;

            SetAnimatorFlip();

            unitAnimator.SetAnimationState(UnitAnimationType.Idle);

            moveCollider.gameObject.SetActive(false);
            fireballWarning.gameObject.SetActive(false);
            foreach (var laser in laserWarning) 
            {
                laser.gameObject.SetActive(false);
            }
        }

        public void OnUpdate()
        {
            float deltaTime = Time.deltaTime;
            if (IsControllable)
            {
                UpdateAttack(deltaTime);
            }

            unitAnimator.UpdateAnimation(deltaTime);
        }

        private void UpdateAttack(float deltaTime)
        {
            if (IsAttacking)
                return;

            if (attackDuration > 0)
            {
                attackDuration -= deltaTime;
                if (attackDuration <= 0)
                {
                    var attackTypes = new List<BossAttackType>((int)BossAttackType.Max - 1);
                    for (var attackType = BossAttackType.Move; attackType < BossAttackType.Max; attackType++)
                    {
                        if (attackType == prevType)
                            continue;

                        attackTypes.Add(attackType);
                    }

                    Attack(attackTypes[Random.Range(0, attackTypes.Count)]);
                }
            }
        }

        private void Attack(BossAttackType patternType)
        {
            prevType = patternType;
            switch (patternType)
            {
                case BossAttackType.Move:
                    attackCoroutine = StartCoroutine(PatternMove());
                    attackDuration += 1;
                    break;
                case BossAttackType.Summon:
                    attackCoroutine = StartCoroutine(PatternSummon());
                    attackDuration += 3;
                    break;
                case BossAttackType.EarthBreak:
                    attackCoroutine = StartCoroutine(PatternEarthBreak());
                    attackDuration += 3.5f;
                    break;
                case BossAttackType.FireBall:
                    attackCoroutine = StartCoroutine(PatternFireBall());
                    attackDuration += 3.5f;
                    break;
                case BossAttackType.Laser:
                    attackCoroutine = StartCoroutine(PatternLaser());
                    attackDuration += 5f;
                    break;
            }
        }

        private IEnumerator PatternLaser()
        {
            IsUsing[BossAttackType.Laser] = true;

            unitMover.IsPassPlatform = true;
            yield return Move(bossTransforms[4].position);

            unitAnimator.ClearPlayState();
            unitAnimator.PlayAnimationClip(UnitAnimationType.Special4);

            laserWarning[0].SetActive(true);
            yield return new WaitForSeconds(2);

            List<Projectile> lasers = new List<Projectile>();

            laserWarning[0].SetActive(false);
            yield return new WaitForSeconds(0.4f);
            var laser = Instantiate(laserProjectile);
            laser.transform.position = laserWarning[0].transform.position + new Vector3(0, -10);
            laser.transform.rotation = Quaternion.identity;
            laser.ProjectileAnimator.PlayAnimationClip(ProjectileAnimationType.Start);
            laser.ProjectileAnimator.SetAnimationState(ProjectileAnimationType.Loop);

            lasers.Add(laser);

            laserWarning[1].SetActive(true);
            laserWarning[2].SetActive(true);
            yield return new WaitForSeconds(2f);

            laserWarning[1].SetActive(false);
            laserWarning[2].SetActive(false);
            yield return new WaitForSeconds(0.4f);
            laser = Instantiate(laserProjectile);
            laser.transform.position = laserWarning[1].transform.position + new Vector3(0, -10);
            laser.transform.rotation = Quaternion.identity;
            laser.ProjectileAnimator.PlayAnimationClip(ProjectileAnimationType.Start);
            laser.ProjectileAnimator.SetAnimationState(ProjectileAnimationType.Loop);

            lasers.Add(laser);

            laser = Instantiate(laserProjectile);
            laser.transform.position = laserWarning[2].transform.position + new Vector3(0, -10);
            laser.transform.rotation = Quaternion.identity;
            laser.ProjectileAnimator.PlayAnimationClip(ProjectileAnimationType.Start);
            laser.ProjectileAnimator.SetAnimationState(ProjectileAnimationType.Loop);

            lasers.Add(laser);

            yield return new WaitForSeconds(2);

            foreach(var disableLaser in lasers)
            {
                disableLaser.ProjectileAnimator.SetAnimationCallBack(ProjectileAnimationType.End, -1, () => disableLaser.gameObject.SetActive(false));
                disableLaser.ProjectileAnimator.PlayAnimationClip(ProjectileAnimationType.End);
            }

            unitMover.IsPassPlatform = false;
            yield return new WaitForSeconds(2);

            CancelAttack();
        }
        private IEnumerator CreateLaser(int index)
        {
            laserWarning[index].SetActive(false);
            yield return new WaitForSeconds(0.4f);
            var laser = Instantiate(laserProjectile);
            laser.transform.position = laserWarning[index].transform.position + new Vector3(0, -10);
            laser.transform.rotation = Quaternion.identity;
            laser.ProjectileAnimator.PlayAnimationClip(ProjectileAnimationType.Start);
            laser.ProjectileAnimator.SetAnimationState(ProjectileAnimationType.Loop);
        }

        private IEnumerator PatternFireBall()
        {
            IsUsing[BossAttackType.FireBall] = true;

            CameraManager.Instance.Shake(0.2f, 3f, 2f);

            fireballWarning.SetActive(true);

            SoundManager.Instance.PlaySoundSfx("Enemy_Dog2", 0.6f, 2f);
            SoundManager.Instance.PlaySoundSfx("Enemy_Bird", 1, 1);
            yield return new WaitForSeconds(0.2f);
            SoundManager.Instance.PlaySoundSfx("Enemy_Bird", 1, 1.2f);
            yield return new WaitForSeconds(0.2f);
            SoundManager.Instance.PlaySoundSfx("Enemy_Bird", 1, 1.4f);
            yield return new WaitForSeconds(1);

            unitAnimator.ClearPlayState();
            unitAnimator.PlayAnimationClip(UnitAnimationType.Walk);

            unitMover.IsPassPlatform = true;

            var trans = Random.Range(0, 2) == 0 ? fireballTransform : fireballTransform.Reverse(); 
            foreach (var fireball in trans)
            {
                yield return Move(fireball.position);
                fireballWarning.SetActive(false);
                
                yield return new WaitForSeconds(0.4f);
                
                SoundManager.Instance.PlaySoundSfx("Enemy_Human", 1, 1.2f);
                
                var dir = (GameManager.Instance.playerUnit.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                var obj = Instantiate(fireballProjectile);
                obj.gameObject.SetActive(true);
                obj.velocity = dir;
                obj.transform.position = transform.position;
                obj.transform.rotation = Quaternion.Euler(0, 0, angle + 180);

                CameraManager.Instance.Shake(0.2f, 3f, 2f);
                fireballWarning.SetActive(true);
            }

            unitMover.IsPassPlatform = false;
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);
            CancelAttack();
        }

        private IEnumerator PatternEarthBreak()
        {
            IsUsing[BossAttackType.EarthBreak] = true;

            SoundManager.Instance.PlaySoundSfx("Enemy_Growl", 0.6f, 2f);

            CameraManager.Instance.Shake(0.2f, 3f, 2f);

            unitAnimator.ClearPlayState();
            unitAnimator.PlayAnimationClip(UnitAnimationType.Walk);

            var playerPosition = GameManager.Instance.playerUnit.transform.position;
            lastEarthBreakPlayerPosition = playerPosition;

            earthBreakWarning.transform.position = lastEarthBreakPlayerPosition + new Vector3(0, 3.5f, 0);
            earthBreakWarning.SetActive(true);

            SoundManager.Instance.PlaySoundAmbient("Enemy_Warning", 0.6f);

            unitMover.IsPassPlatform = true;
            yield return Move(lastEarthBreakPlayerPosition);
            unitMover.IsPassPlatform = false;

            earthBreakWarning.SetActive(false);

            yield return new WaitForSeconds(0.4f);

            unitAnimator.PlayAnimationClip(UnitAnimationType.Special4);
            unitAnimator.SetAnimationState(UnitAnimationType.Idle);

            yield return new WaitForSeconds(2);

            CancelAttack();
        }

        private IEnumerator PatternMove()
        {
            IsUsing[BossAttackType.Move] = true;

            moveCollider.gameObject.SetActive(true);
            moveCollider.ClearColliders();

            bossTransforms.Shuffle();
            unitAnimator.ClearPlayState();
            for (int i = 0; i < bossTransforms.Length; i++)
            {
                CameraManager.Instance.Shake(0.2f, 3f, 2f);
                unitAnimator.PlayAnimationClip(UnitAnimationType.Special2);
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

            SoundManager.Instance.PlaySoundSfx("Enemy_Bird", 1, 1.2f);

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

            SetAnimatorFlip();
        }
    }
}