using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class UnitAnimator : MonoBehaviour
    {
        private int animStateHash;

        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void ChangeAnimState(int animationHash, float fixedDuration = 0.5f)
        {
            if (animStateHash == animationHash) return;

            animStateHash = animationHash;
            animator.Play(animationHash);
        }

        public void ChangeFlip(bool isFlip)
        {
            transform.rotation = isFlip ? Quaternion.Euler(0, 180, 0) : Quaternion.identity; 
        }
    }
}