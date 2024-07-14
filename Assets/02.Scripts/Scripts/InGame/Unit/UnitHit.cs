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
        public float Hp { get; private set; }
        [FormerlySerializedAs("MaxHp")]
        [SerializeField] private float maxHp;

        private Action dieAction;

        private void Awake()
        {
            Hp = maxHp;
        }

        public void Hit(float damage)
        {
            Hp -= damage;
            if (Hp > 0) return;

            Die();
        }

        public void SetDieAction(Action action)
        {
            dieAction = action;
        }

        private void Die()
        {
            gameObject.SetActive(false);
            dieAction?.Invoke();
        }
    }

}