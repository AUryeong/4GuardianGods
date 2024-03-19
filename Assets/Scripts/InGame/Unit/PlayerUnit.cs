using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class PlayerUnit : Unit
    {
        protected override void Update()
        {
            UpdateVelocity();
            UpdateAnimation();
            unitMover.Move();
        }

        private void UpdateVelocity()
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            unitMover.velocity.x += inputX;

            if (Input.GetKeyDown(KeyCode.W))
            {
                unitMover.Rigid.velocity = Vector2.up * 10;
                unitAnimator.ChangeAnimState(UnitAnimationType.Jump);
            }
        }
    }
}