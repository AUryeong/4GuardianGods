using InGame.Unit;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InGame
{
    public class Map : SerializedMonoBehaviour
    {
        public PolygonCollider2D mapCollider;
        public List<Enemy> enemies;

        private Tilemap tileMap;
        private TilemapRenderer tileMapRenderer;

        private void Awake()
        {
            if(mapCollider == null)
                mapCollider = GetComponent<PolygonCollider2D>();

            if (tileMap == null)
            {
                tileMap = GetComponent<Tilemap>();
            }

            if (tileMapRenderer == null)
            {
                tileMapRenderer = GetComponent<TilemapRenderer>();
            }
        }

        public virtual void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
            if (!GameManager.Instance.playerUnit.IsControllable)
                return;
            
            foreach(var enemy in enemies)
            {
                if (!enemy)
                    continue;
                
                if (!enemy.gameObject.activeSelf)
                    continue;
                
                enemy.OnFixedUpdate();
            }
        }

        public virtual void OnEnter()
        {
            foreach (var enemy in enemies)
            {
                if (!enemy)
                    continue;
                
                enemy.OnEnter();
            }
        }

        public virtual void OnExit()
        {
            foreach (var enemy in enemies)
            {
                if (!enemy)
                    continue;
                
                enemy.OnExit();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                CameraManager.Instance.Enqueue(this);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                CameraManager.Instance.DeQueue(this);
            }
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Enemy>()?.UnitHit?.Die();
            }
        }
    }

}