using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace InGame
{
    public class SpriteAnimator : SerializedMonoBehaviour
    {
        protected const float ANIMATION_TRANSFORM_DELAY = 0.2f;

        [SerializeField] protected SpriteRenderer spriteRenderer;
        public SpriteAnimationClip PlayClip
        {
            get
            {
                return playClip == null ? playStateClip : playClip;
            }
        }

        protected SpriteAnimationClip playClip;
        protected SpriteAnimationClip playStateClip;

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

        public void SetAnimationClip(ref SpriteAnimationClip refClip, SpriteAnimationClip animClip, bool isResetFrame = true)
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
            if (!PlayClip) return;

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