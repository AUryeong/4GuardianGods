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
        Special
    }

    public class UnitAnimator : SerializedMonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        
        [Header("Value")]
        [SerializeField] protected Dictionary<UnitAnimationType, UnitAnimationClip> animationDict = new(5)
        {
            { UnitAnimationType.Idle, null },
            { UnitAnimationType.Walk, null },
            { UnitAnimationType.Jump, null },
            { UnitAnimationType.Fall, null }
        };

        private UnitAnimationClip playClip;

        private float frame;
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

        public virtual void SetAnimation(UnitAnimationType type, bool isResetFrame = true)
        {
            SetAnimationClip(animationDict[type], isResetFrame);
        }

        public void SetAnimationClip(UnitAnimationClip animClip, bool isResetFrame = true)
        {
            if (animClip == playClip) return;
            if (animClip == null) return;

            playClip = animClip;
            if (isResetFrame)
                frame = 0;
            else
                frame %= animClip.MaxFrame;
        }

        public void UpdateAnimation()
        {
            frame += Time.deltaTime * playClip.OneSecondPerFrame;

            if (frame < 0.2f) return;

            if (frame >= playClip.MaxFrame)
            {
                if (playClip.isLoop)
                    frame = 0;
                else
                    return;
            }

            spriteRenderer.sprite = playClip.GetSprite((int)frame);
        }
    }
}