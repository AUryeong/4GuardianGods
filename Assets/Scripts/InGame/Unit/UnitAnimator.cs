using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using InGame;
using UnityEngine;

namespace InGame.Unit
{
    public enum UnitAnimationType
    {
        Idle,
        Walk,
        Jump,
        Fall,
        Special,
        Special2,
        Special3
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

        public virtual void SetAnimationState(UnitAnimationType type, bool isResetFrame = true)
        {
            if (animationDict.TryGetValue(type, out var animationClip))
                SetAnimationClip(ref playStateClip, animationClip, isResetFrame);
        }

        public void PlayAnimationClip(UnitAnimationType type, bool isResetFrame = true)
        {
            if (animationDict.TryGetValue(type, out var animationClip))
                SetAnimationClip(ref playClip, animationClip, isResetFrame);
        }
    }
}