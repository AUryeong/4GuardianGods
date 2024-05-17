using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class PlayerUnit : Unit
    {
        public bool IsDrawing
        {
            get
            {
                return Input.GetMouseButton(0);
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateVelocity();
            UpdateAnimState();
            unitAnimator.UpdateAnimation(deltaTime);
            unitMover.UpdateMove(deltaTime);
        }

        private void UpdateVelocity()
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            unitMover.velocity.x += inputX;

            if (Input.GetKeyDown(KeyCode.W))
            {
                unitMover.Rigid.velocity = Vector2.up * 10;
                unitAnimator.PlayAnimationClip(UnitAnimationType.Jump);
            }
        }
    }
}