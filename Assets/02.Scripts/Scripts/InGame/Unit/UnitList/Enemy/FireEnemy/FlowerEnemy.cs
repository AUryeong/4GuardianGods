using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class FlowerEnemy : Enemy
    {
        [SerializeField] private WarningObject targetWarning;
        [SerializeField] private Projectile projectile;

        private Vector3 lastPlayerPosition;

        protected override void Start()
        {
            base.Start();
            UnitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 7, Attack);
            targetWarning.gameObject.SetActive(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            targetWarning.SetActive(false);
        }

        public override void OnExit()
        {
            base.OnExit();
            targetWarning.SetActive(false);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (attackDuration < attackCooltime / 5)
            {
                targetWarning.SetActive(true);
                if (GameManager.Instance.playerUnit.UnitMover.IsGround)
                {
                    targetWarning.transform.position = GameManager.Instance.playerUnit.transform.position;
                }
            }
        }

        protected override void PlayAttack()
        {
            if (!GameManager.Instance.playerUnit.UnitMover.IsGround)
                return;

            lastPlayerPosition = GameManager.Instance.playerUnit.transform.position;
            targetWarning.transform.position = lastPlayerPosition;
            targetWarning.SetActive(false);
            base.PlayAttack();
        }

        protected override void Attack()
        {
            base.Attack();

            CameraManager.Instance.Shake(0.2f, 2f, 3f);

            var obj = Instantiate(projectile);
            obj.transform.position = lastPlayerPosition;
        }
    }
}