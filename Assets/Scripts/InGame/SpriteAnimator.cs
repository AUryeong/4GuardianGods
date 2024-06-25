using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class SpriteAnimator : SerializedMonoBehaviour
    {
        protected const float ANIMATION_TRANSFORM_DELAY = 0.2f;
        protected Dictionary<SpriteAnimationClip, Dictionary<int, Action>> animationCallBack;

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
                spriteRenderer.transform.localScale = isFlip ? new Vector3(-1,1,1) : Vector3.one;
            }
        }

        private bool isFlip;

        public void SetAnimationCallBack(SpriteAnimationClip clip, int frame, Action action)
        {
            if(animationCallBack == null)
            {
                animationCallBack = new Dictionary<SpriteAnimationClip, Dictionary<int, Action>>();
            }
            if(!animationCallBack.TryGetValue(clip, out var frameDict))
            {
                frameDict = new Dictionary<int, Action>();
                animationCallBack.Add(clip, frameDict);
            }

            if (!frameDict.TryGetValue(frame, out var frameAction))
            {
                frameAction = action;
                frameDict.Add(frame, frameAction);
            }
            else
            {
                frameDict[frame] = action;
            }
        }

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
            if (PlayClip == null) return;

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

            spriteRenderer.sprite = PlayClip.GetSprite(Frame);
            if (animationCallBack != null)
            {
                if (animationCallBack.TryGetValue(PlayClip, out var actionDict))
                {
                    if (actionDict.TryGetValue(Frame, out var action))
                    {
                        action?.Invoke();
                    }
                }
            }
        }
    }

}