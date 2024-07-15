using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class DefaultEnemy : Enemy
    {
        protected override void Start()
        {
            base.Start();
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 4, Attack);
        }

        protected override void Attack()
        {
            base.Attack();

        }
    }
}