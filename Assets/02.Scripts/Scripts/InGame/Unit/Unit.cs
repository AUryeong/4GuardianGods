using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Unit
{
    public enum UnitType
    {
        None,
        Enemy,
        Player
    }
    public class Unit : PoolableUnit<Unit>
    {
        public UnitMover UnitMover => unitMover;
        [SerializeField] protected UnitMover unitMover;
        
        public UnitAnimator UnitAnimator => unitAnimator;
        [SerializeField] protected UnitAnimator unitAnimator;
        
        public UnitHit UnitHit => unitHit;
        [SerializeField] protected UnitHit unitHit;

        protected virtual void UpdateAnimState()
        {
            if (unitMover.velocity.x != 0)
                unitAnimator.IsFlip = unitMover.velocity.x > 0;

            if (unitMover.Rigid.velocity.y > 0) return;

            if (unitMover.Rigid.velocity.y < 0)
                unitAnimator.SetAnimationState(UnitAnimationType.Fall);
            else
                unitAnimator.SetAnimationState(unitMover.velocity.x == 0 ? UnitAnimationType.Idle : UnitAnimationType.Walk);
        }
    }
}