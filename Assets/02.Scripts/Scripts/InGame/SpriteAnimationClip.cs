using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(fileName = "AnimationClip", menuName = "Animation", order = 0)]
    public class SpriteAnimationClip : ScriptableObject
    {
        [SerializeField] private Sprite[] animations;

        public bool isLoop = true;
        [SerializeField] private float oneSecondPerFrame = 12;
        public float OneSecondPerFrame => oneSecondPerFrame;

        public int MaxFrame => animations.Length;

        public Sprite GetSprite(int frame)
        {
            Debug.Assert(frame < MaxFrame);
            return animations[frame];
        }
    }
}