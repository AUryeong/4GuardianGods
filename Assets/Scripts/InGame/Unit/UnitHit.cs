using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Unit
{
    public class UnitHit : MonoBehaviour
    {
        [SerializeField] protected PoolableUnit<Unit> poolUnit;

        public float Hp { get; private set; }
        [SerializeField] private float MaxHp;

        public ParticleSystem dieEffect;

        private void Awake()
        {
            Hp = MaxHp;
        }



        private void OnCollisionEnter2D(Collision2D collision)
        {
            var projecttile = collision.collider.GetComponent<Projectile>();
            if (projecttile == null)
                return;

            Hp -= projecttile.damage;
            transform.DOShakePosition(0.3f, 0.4f);
            if (Hp <= 0)
            {
                gameObject.SetActive(false);
                var obj = Instantiate(dieEffect);
                obj.transform.position = transform.position;
                if (!poolUnit)
                    poolUnit.PushPool();
                return;
            }
        }
    }

}