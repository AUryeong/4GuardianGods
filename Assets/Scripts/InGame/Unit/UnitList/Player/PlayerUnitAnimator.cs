using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class PlayerUnitAnimator : UnitAnimator
    {
        [Header("Brush Animation")]
        [SerializeField]
        protected Dictionary<UnitAnimationType, UnitAnimationClip> brushAnimationDict = new(5)
        {
            { UnitAnimationType.Idle, null },
            { UnitAnimationType.Walk, null },
            { UnitAnimationType.Jump, null },
            { UnitAnimationType.Fall, null }
        };

        private bool lastDrawing = false;

        public override void SetAnimation(UnitAnimationType type, bool isResetFrame = true)
        {
            bool isDrawing = GameManager.Instance.playerUnit.IsDrawing;
            isResetFrame = lastDrawing == isDrawing;
            lastDrawing = isDrawing;

            if (!isResetFrame)
                Debug.Log(type);

            if (isDrawing)
                SetAnimationClip(brushAnimationDict[type], isResetFrame);
            else
                SetAnimationClip(animationDict[type], isResetFrame);
        }
    }
}
