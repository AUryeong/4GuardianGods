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
            bool isDrawing = DrawManager.Instance.IsDrawing;
            isResetFrame = lastDrawing == isDrawing;
            lastDrawing = isDrawing;

            SpriteAnimationClip animClip;
            if (isDrawing)
            {
                if (brushAnimationDict.TryGetValue(type, out animClip))
                {
                    SetAnimationClip(ref playStateClip, animClip, isResetFrame);
                    return;
                }
            }
            if (animationDict.TryGetValue(type, out animClip))
            {
                SetAnimationClip(ref playStateClip, animClip, isResetFrame);
            }
        }

        public override void PlayAnimationClip(UnitAnimationType type, bool isResetFrame = true)
        {
            if (DrawManager.Instance.IsDrawing)
            {
                if (brushAnimationDict.TryGetValue(type, out var animClip))
                {
                    SetAnimationClip(ref playClip, animClip, isResetFrame);
                }
            }
            else
            {
                if (animationDict.TryGetValue(type, out var animClip))
                {
                    SetAnimationClip(ref playClip, animClip, isResetFrame);
                }
            }
        }
    }
}
