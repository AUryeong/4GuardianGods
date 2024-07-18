using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace InGame.Unit
{
    public class PlayerUnit : Unit
    {
        public bool IsControllable
        {
            get
            {
                return isControllable;
            }
            set
            {
                isControllable = value;
                if (!value)
                {
                    UnitMover.Rigid.velocity = Vector2.zero;
                }
                dashDuration = 0;
                isRolling = false;
                unitAnimator.SpriteRenderer.transform.rotation = Quaternion.identity;
                ResetJump();
            }
        }
        private bool isControllable = true;
        public bool IsWallStanding
        {
            get { return isWallStanding; }
            set
            {
                isWallStanding = value;
                if (isWallStanding)
                {
                    unitMover.Rigid.velocity = Vector2.zero;
                }

                UnitMover.Rigid.gravityScale = isWallStanding ? 0 : 2.5f;
            }
        }
        private bool isWallStanding = false;

        [SerializeField] private bool isRolling = false;

        [Header("Dash")]
        private bool isUsedDash = false;
        private const float DASH_DURATION = 0.2f;

        private bool IsDashing => dashDuration > 0;
        private Vector2 dashDirection;
        private float dashDuration;

        private int jumpCount = 0;
        private const int MAX_JUMP_COUNT = 2;
        private const float JUMP_POWER = 10;
        private const float DASH_SPEED = 5;

        private void Awake()
        {
            unitHit.SetHitAction(HitAction);
            unitHit.SetDieAction(DieAction);
        }

        private void HitAction()
        {
            UnitAnimator.Hit();

            UIManager.Instance.Hit();
        }

        private void DieAction()
        {
            if (!IsControllable)
                return;
            IsControllable = false;
            isRolling = false;
            ResetJump();

            UnitAnimator.Hit();

            UIManager.Instance.Die();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            if (IsControllable)
            {
                CheckInput();
            }
            else
            {
                SoundManager.Instance.StopSoundAmbient("Step_Grass");
            }

            if (isWallStanding)
            {
                unitMover.Rigid.velocity = Vector2.zero;
            }

            UpdateAnimState();
            unitAnimator.UpdateAnimation(deltaTime);
            unitMover.UpdateMove(deltaTime);
        }

        private void CheckInput()
        {
            CheckDown();
            CheckJump();
            CheckDash();
            UpdateVelocity();
        }

        private void CheckDown()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var vector = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                vector.z = 0;
                transform.position =vector;
            }
            if (Input.GetKeyDown(KeyCode.S) && IsWallStanding)
            {
                IsWallStanding = false;
            }
        }

        private void CheckDash()
        {
            if (IsDashing)
            {
                dashDuration -= Time.deltaTime;
                if (UnitMover.Move(DASH_SPEED * Time.deltaTime / DASH_DURATION * dashDirection, CheckCollider))
                {
                    if (!IsDashing)
                    {
                        isRolling = true;

                        unitMover.Rigid.velocity = DASH_SPEED * 2 * dashDirection;

                        unitAnimator.SpriteRenderer.transform.rotation = Quaternion.identity;
                        unitAnimator.IsFlip = dashDirection.x > 0;
                        unitAnimator.SetAnimationState(UnitAnimationType.Special3);
                    }
                }
            }

            if (isUsedDash)
                return;

            if (Input.GetMouseButtonDown(1))
            {
                isUsedDash = true;
                if (IsWallStanding)
                    IsWallStanding = false;

                var pos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                var direction = ((Vector2)(pos - transform.position)).normalized;

                dashDuration = DASH_DURATION;
                dashDirection = direction;

                SoundManager.Instance.PlaySoundSfx("Dash", 1.5f, 3);
                SoundManager.Instance.PlaySoundSfx("Dash2", 1f, 2);
                SoundManager.Instance.PlaySoundSfx("Dash3", 0.5f, 1);

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                unitAnimator.IsFlip = false;
                unitAnimator.SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);

                CameraManager.Instance.Shake(0.1f, 1f, 5f);
            }
        }

        private void CheckCollider(RaycastHit2D rayCastHit)
        {
            dashDuration = 0;

            transform.position = rayCastHit.point;
            unitAnimator.SpriteRenderer.transform.rotation = Quaternion.identity;
            if (rayCastHit.collider.CompareTag("Platform"))
            {
                if (!unitMover.GetGround(dashDirection.y * DASH_SPEED))
                {
                    unitAnimator.SetAnimationState(UnitAnimationType.Special);
                    UnitAnimator.IsFlip = dashDirection.x > 0;

                    IsWallStanding = true;
                }

                ResetJump();
            }
            else if (rayCastHit.collider.CompareTag("Brush"))
            {
                isRolling = true;

                unitAnimator.SetAnimationState(UnitAnimationType.Special3);
                
                SoundManager.Instance.PlaySoundSfx("Hit_Brush", 1.5f, 1);
                SoundManager.Instance.PlaySoundSfx("Hit_Brush2", 0.6f, 0.5f);
                SoundManager.Instance.PlaySoundSfx("Hit_Brush4", 1, 0.8f);

                var drawing = rayCastHit.collider.GetComponent<Drawing>();
                drawing.Type = DrawingType.Projectile;
                drawing.Throw(dashDirection);

                unitMover.Rigid.velocity = -DASH_SPEED * Mathf.Sqrt(drawing.Power) * 2 * dashDirection;
                unitMover.velocity = Vector2.zero;

                ResetJump();
            }
        }

        private void ResetJump()
        {
            jumpCount = 0;
            isUsedDash = false;
        }

        private void CheckJump()
        {
            if (unitMover.Rigid.velocity.y <= 0)
            {
                unitMover.CheckGround();
                if (unitMover.IsGround && !IsDashing && (isRolling || jumpCount > 0 || isUsedDash))
                {
                    ResetJump();
                    SoundManager.Instance.PlaySoundSfx("Fall_Grass", 1.25f, 0.75f);
                    SoundManager.Instance.PlaySoundSfx("Fall_Grass2", 1.25f, 1);
                    isRolling = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (jumpCount < MAX_JUMP_COUNT)
                {
                    IsWallStanding = false;
                    isRolling = false;
                    jumpCount++;
                    unitMover.Rigid.velocity = Vector2.up * JUMP_POWER;
                    unitAnimator.PlayAnimationClip(UnitAnimationType.Jump);
                }
            }
        }

        protected override void UpdateAnimState()
        {
            if (IsWallStanding)
            {
                unitAnimator.SetAnimationState(UnitAnimationType.Special);
                return;
            }

            if (IsDashing)
            {
                unitAnimator.SetAnimationState(UnitAnimationType.Special2);
                return;
            }

            if (isRolling)
                return;

            base.UpdateAnimState();
        }

        private void UpdateVelocity()
        {
            if (IsWallStanding)
            {
                SoundManager.Instance.StopSoundAmbient("Step_Grass");
                return;
            }

            float inputX = Input.GetAxisRaw("Horizontal");
            if (inputX != 0)
            {
                unitMover.velocity.x += inputX;
                if (jumpCount == 0 && UnitMover.IsGround && unitAnimator.IsPlayAnimation(UnitAnimationType.Walk))
                {
                    SoundManager.Instance.PlaySoundAmbient("Step_Grass", 1, 1.2f);
                    return;
                }
            }

            SoundManager.Instance.StopSoundAmbient("Step_Grass");
        }
    }
}