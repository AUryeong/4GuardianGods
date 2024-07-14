using InGame.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InGame
{
    public class Map : MonoBehaviour
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

        public void OnFixedUpdate()
        {
            foreach(var enemy in enemies)
            {
                enemy.OnFixedUpdate();
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
        }
    }

}