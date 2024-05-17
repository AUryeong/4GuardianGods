using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

    public class UnitAnimator : SerializedMonoBehaviour
    {
        protected const float ANIMATION_TRANSFORM_DELAY = 0.2f;

        [SerializeField] protected SpriteRenderer spriteRenderer;

        [Header("Value")]
        [SerializeField]
        protected Dictionary<UnitAnimationType, UnitAnimationClip> animationDict = new(5)
        {
            { UnitAnimationType.Idle, null },
            { UnitAnimationType.Walk, null },
            { UnitAnimationType.Jump, null },
            { UnitAnimationType.Fall, null }
        };

        public UnitAnimationClip PlayClip
        {
            get 
            {
                return playClip == null ? playStateClip : playClip;
            }
        }

        protected UnitAnimationClip playClip;
        protected UnitAnimationClip playStateClip;

        protected float frame;
        public int Frame => (int)frame;

        public bool IsFlip
        {
            set
            {
                if (isFlip == value)
                    return;

                isFlip = value;
                spriteRenderer.transform.rotation = isFlip ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
            }
        }

        private bool isFlip;

        public Action startCallBack;
        public Action endCallBack;

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

        public void SetAnimationClip(ref UnitAnimationClip refClip, UnitAnimationClip animClip, bool isResetFrame = true)
        {
            if (animClip == null) return;
            if (animClip == refClip) return;

            refClip = animClip;
            if (playClip != null && playClip != refClip) return;

            if (isResetFrame)
                frame = 0;
            else
                frame %= animClip.MaxFrame;
        }

        public void UpdateAnimation(float deltaTime)
        {
            frame += deltaTime * PlayClip.OneSecondPerFrame;

            if (frame < ANIMATION_TRANSFORM_DELAY) return;

            if (frame >= PlayClip.MaxFrame)
            {
                frame = 0;
                if (PlayClip == playClip)
                {
                    playClip = null;
                    return;
                }
            }

            spriteRenderer.sprite = PlayClip.GetSprite((int)frame);
        }
    }
}