using UnityEngine;

namespace InGame.Unit
{
    public class HumanEnemy : ProjectileEnemy
    {
        protected override void Start()
        {
            base.Start();
            unitAnimator.SetAnimationCallBack(UnitAnimationType.Special, 4, Attack);
        }
    }
}