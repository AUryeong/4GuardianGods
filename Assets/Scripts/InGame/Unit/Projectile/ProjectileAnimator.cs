using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public enum ProjectileAnimationType
    {
        Start,
        Loop,
        End,
        Special
    }
    public class ProjectileAnimator : SpriteAnimator
    {
        [Header("Value")]
        [SerializeField]
        protected Dictionary<ProjectileAnimationType, SpriteAnimationClip> animationDict = new(5)
        {
            { ProjectileAnimationType.Start, null },
            { ProjectileAnimationType.Loop, null },
            { ProjectileAnimationType.End, null },
        };

        public SpriteAnimationClip GetSpriteAnimationClip(ProjectileAnimationType type)
        {
            return animationDict[type];
        }
        public void SetAnimationCallBack(ProjectileAnimationType type, int frame, Action action)
        {
            if (!animationDict.TryGetValue(type, out var animationClip))
                return;
            SetAnimationCallBack(animationClip, frame, action);
        }

        public virtual void SetAnimationState(ProjectileAnimationType type, bool isResetFrame = true)
        {
            if (animationDict.TryGetValue(type, out var animationClip))
                SetAnimationClip(ref playStateClip, animationClip, isResetFrame);
        }

        public void PlayAnimationClip(ProjectileAnimationType type, bool isResetFrame = true)
        {
            if (animationDict.TryGetValue(type, out var animationClip))
                SetAnimationClip(ref playClip, animationClip, isResetFrame);
        }
    }
}