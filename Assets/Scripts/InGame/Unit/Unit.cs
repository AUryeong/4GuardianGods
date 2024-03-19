using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Unit
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] protected UnitMover unitMover;
        [SerializeField] protected UnitAnimator unitAnimator;

        protected virtual void Update()
        {
            UpdateAnimation();
        }

        protected void UpdateAnimation()
        {
            if (unitMover.velocity.x != 0)
                unitAnimator.ChangeFlip(unitMover.velocity.x > 0);

            if (unitMover.Rigid.velocity.y > 0) return;

            if (unitMover.Rigid.velocity.y < 0)
                unitAnimator.ChangeAnimState(UnitAnimationType.Fall);
            else
                unitAnimator.ChangeAnimState((unitMover.velocity.x == 0) ? UnitAnimationType.Idle : UnitAnimationType.Walk);
        }
    }
}