using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private int playAnimHash;
        private int nextAnimStateHash;

        [SerializeField]
        private Dictionary<UnitAnimationType, AnimationClip> animationDict = new Dictionary<UnitAnimationType, AnimationClip>(5)
        {
            {UnitAnimationType.Idle, null},
            {UnitAnimationType.Walk, null},
            {UnitAnimationType.Jump, null},
            {UnitAnimationType.Fall, null}
        };

        private Dictionary<UnitAnimationType, int> animationHashDict;
        private Dictionary<UnitAnimationType, int> AnimationHashDict
        {
            get
            {
                if (animationHashDict == null)
                {
                    animationHashDict = new Dictionary<UnitAnimationType, int>(5);
                    foreach (var pair in animationDict)
                        animationHashDict.Add(pair.Key, Animator.StringToHash(pair.Value.name));
                }
                return animationHashDict;
            }
        }

        public void Play(int animationHash)
        {
            playAnimHash = animationHash;
            animator.Play(animationHash);
        }

        public void ChangeAnimState(UnitAnimationType type)
        {
            ChangeAnimState(AnimationHashDict[type]);
        }

        public void ChangeAnimState(int animationHash)
        {
            if (nextAnimStateHash == animationHash) return;
            if (playAnimHash > 0)
            {
                nextAnimStateHash = animationHash;
                return;
            }

            animator.Play(animationHash);
        }

        public void ChangeFlip(bool isFlip)
        {
            transform.rotation = isFlip ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
    }
}