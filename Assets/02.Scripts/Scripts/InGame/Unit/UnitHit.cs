using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class UnitHit : MonoBehaviour
    {
        public float Hp;
        [FormerlySerializedAs("MaxHp")]
        [SerializeField] private float maxHp;

        private Action hitAction;
        private Action dieAction;

        private void Awake()
        {
            Hp = maxHp;
        }

        public virtual void Hit(float damage)
        {
            Hp -= damage;
            hitAction?.Invoke();
            if (Hp > 0) return;

            Die();
        }

        public void SetDieAction(Action action)
        {
            dieAction = action;
        }

        public void SetHitAction(Action action)
        {
            hitAction = action;
        }

        public void Die()
        {
            Hp = 0;
            if (dieAction == null)
                Destroy(gameObject);
            else
                dieAction.Invoke();
        }

        public void SetHpMax()
        {
            Hp = maxHp;
        }
    }
}