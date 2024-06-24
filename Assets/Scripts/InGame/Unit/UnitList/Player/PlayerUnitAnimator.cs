using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class PlayerUnitAnimator : UnitAnimator
    {
        [Header("Brush Animation")]
        [SerializeField]
        protected Dictionary<UnitAnimationType, SpriteAnimationClip> brushAnimationDict = new(5)
        {
            { UnitAnimationType.Idle, null },
            { UnitAnimationType.Walk, null },
            { UnitAnimationType.Jump, null },
            { UnitAnimationType.Fall, null }
        };

        private bool lastDrawing = false;

        public override void SetAnimationState(UnitAnimationType type, bool isResetFrame = true)
        {
            bool isDrawing = GameManager.Instance.playerUnit.IsDrawing;
            isResetFrame = lastDrawing == isDrawing;
            lastDrawing = isDrawing;

            if (isDrawing)
                SetAnimationClip(ref playStateClip, brushAnimationDict[type], isResetFrame);
            else
                SetAnimationClip(ref playStateClip, animationDict[type], isResetFrame);
        }
    }
}
