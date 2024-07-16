using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using InGame;
using UnityEngine;

namespace InGame.Unit
{
    public enum UnitAnimationType
    {
        None = -1,
        Idle,
        Walk,
        Jump,
        Fall,
        Special,
        Special2,
        Special3,
        Special4,
        Special5,
        Special6,
        Special7,
    }

    public class UnitAnimator : SpriteAnimator
    {
        [Header("Value")]
        [SerializeField]
        protected Dictionary<UnitAnimationType, SpriteAnimationClip> animationDict = new(5)
        {
            { UnitAnimationType.Idle, null },
            { UnitAnimationType.Walk, null },
            { UnitAnimationType.Jump, null },
            { UnitAnimationType.Fall, null }
        };
        public SpriteAnimationClip GetSpriteAnimationClip(UnitAnimationType type)
        {
            return animationDict[type];
        }

        public bool IsPlayAnimation(UnitAnimationType type = UnitAnimationType.None)
        {
            if (type == UnitAnimationType.None)
                return PlayClip;

            if (!animationDict.TryGetValue(type, out var animationClip)) 
                return false;
            
            if (animationClip != PlayClip)
                return false;

            return true;
        }

        public void SetAnimationCallBack(UnitAnimationType type, int frame, Action action)
        {
            if (!animationDict.TryGetValue(type, out var animationClip))
                return;
            SetAnimationCallBack(animationClip, frame, action);
        }

        public virtual void SetAnimationState(UnitAnimationType type, bool isResetFrame = true)
        {
            if (animationDict.TryGetValue(type, out var animationClip))
                SetAnimationClip(ref playStateClip, animationClip, isResetFrame);
        }

        public virtual void PlayAnimationClip(UnitAnimationType type, bool isResetFrame = true)
        {
            if (animationDict.TryGetValue(type, out var animationClip))
                SetAnimationClip(ref playClip, animationClip, isResetFrame);
        }
    }
}