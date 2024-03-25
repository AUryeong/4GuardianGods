using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGame.Unit
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class UnitMover : MonoBehaviour
    {
        [SerializeField] private Collider2D unitCollider;
        [SerializeField] private Rigidbody2D rigid;

        [Header("Value")]
        public Vector2 velocity;
        public float speed;
        [Range(2, 15)]
        public int horizontalLayCount = 2;

        public Rigidbody2D Rigid
        {
            get
            {
                if (rigid == null)
                    rigid = GetComponent<Rigidbody2D>();
                return rigid;
            }
        }

        public void Move()
        {
            if (velocity == Vector2.zero) return;

            float speedMultiplier = Time.deltaTime * speed;
            var moveDir = velocity * speedMultiplier;

            var centerPivot = unitCollider.bounds.center;
            var size = unitCollider.bounds.size;

            float spacing = size.y / (horizontalLayCount);
            float rayCastDistance = Mathf.Abs(moveDir.x) + size.x / 2;

            for (int i = 0; i < horizontalLayCount; i++)
            {
                Vector2 raycast = new Vector3(centerPivot.x, i * spacing - size.y / 2 + centerPivot.y + spacing);
                RaycastHit2D ray = Physics2D.Raycast(raycast, velocity, rayCastDistance, LayerMask.GetMask("Platform"));
                if (ray.collider != null)
                {
                    velocity = Vector2.zero;
                    return;
                }
                transform.Translate(moveDir, Space.World);
            }
            velocity = Vector2.zero;
        }
    }
}