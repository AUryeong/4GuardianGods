using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class ProjectileEnemy : Enemy
    {
        [SerializeField] private SpriteRenderer target;

        protected override void Start()
        {
            base.Start();
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 4, Attack);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        protected override void Attack()
        {
            base.Attack();

        }
    }
}