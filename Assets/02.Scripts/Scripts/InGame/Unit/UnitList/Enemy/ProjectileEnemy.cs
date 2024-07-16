using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class ProjectileEnemy : Enemy
    {
        [SerializeField] private WarningObject targetWarning;
        [SerializeField] private Projectile projectile;

        private Vector3 lastPlayerPosition;

        protected override void Start()
        {
            base.Start();
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 1, () =>
            {
                lastPlayerPosition = GameManager.Instance.playerUnit.transform.position;
                targetWarning.SetActive(false);
            });
            
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
            if (attackDuration > attackCooltime / 5)
            {
                targetWarning.SetActive(true);
            }
        }

        protected override void Attack()
        {
            base.Attack();
            
            CameraManager.Instance.Shake(0.2f, 2f,3f);
            
            var dir = (lastPlayerPosition - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            var obj = Instantiate(projectile);
            obj.velocity = dir;
            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.Euler(0, 0, angle+180);
        }
    }
}