using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    [CreateAssetMenu(fileName = "AnimationClip", menuName = "Unit/Animation", order = 0)]
    public class UnitAnimationClip : ScriptableObject
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