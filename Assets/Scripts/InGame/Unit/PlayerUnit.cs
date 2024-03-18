using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class PlayerUnit : Unit
    {
        private void Update()
        {
            UpdateVelocity();
            Move();
            UpdateAnimation();
        }

        private void UpdateVelocity()
        {
            velocity = Vector2.zero;

            float inputX = Input.GetAxisRaw("Horizontal");
            velocity.x += inputX;
        }

        private void Move()
        {
            if (velocity.x != 0)
                unitMover.Move(velocity.x);
        }

        private void UpdateAnimation()
        {
            if (velocity.y < 0)
            {
                unitAnimator.ChangeAnimState(Animator.StringToHash("Fall"));
                return;
            }

            if (velocity.x == 0)
                unitAnimator.ChangeAnimState(Animator.StringToHash("Idle"));
            else
            {
                unitAnimator.ChangeFlip(velocity.x > 0);
                unitAnimator.ChangeAnimState(Animator.StringToHash("Walk"));
            }
        }

        private Vector2 velocity;
    }
}