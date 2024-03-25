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
            UpdateAnimState();
            unitAnimator.UpdateAnimation();
            unitMover.Move();
        }

        protected void UpdateAnimState()
        {
            if (unitMover.velocity.x != 0)
                unitAnimator.IsFlip = unitMover.velocity.x > 0;

            if (unitMover.Rigid.velocity.y > 0) return;

            if (unitMover.Rigid.velocity.y < 0)
                unitAnimator.SetAnimation(UnitAnimationType.Fall);
            else
                unitAnimator.SetAnimation(unitMover.velocity.x == 0 ? UnitAnimationType.Idle : UnitAnimationType.Walk);
        }
    }
}