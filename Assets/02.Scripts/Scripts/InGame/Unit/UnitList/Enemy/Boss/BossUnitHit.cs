using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class BossUnitHit : UnitHit
    {
        public override void Hit(float damage)
        {
            base.Hit(1);
        }
    }
}