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

        [Header("Value")]
        public float speed;
        [Range(2, 15)]
        public int horizontalLayCount = 2;

        public Rigidbody2D Rigid => rigid ??= GetComponent<Rigidbody2D>();
        private Rigidbody2D rigid;

        public void Move(float speedMultiplier = 1)
        {
            var moveDir = new Vector2(speedMultiplier * Time.deltaTime * speed, 0);

            var centerPivot = unitCollider.bounds.center;
            var size = unitCollider.bounds.size;

            float face = Mathf.Sign(speedMultiplier);
            float spacing = size.y / (horizontalLayCount);
            float rayCastDistance = Mathf.Abs(moveDir.x) + size.x / 2;

            for (int i = 0; i < horizontalLayCount; i++)
            {
                Vector2 raycast = new Vector3(centerPivot.x, i * spacing - size.y / 2 + centerPivot.y + spacing);
                RaycastHit2D ray = Physics2D.Raycast(raycast, face * Vector2.right, rayCastDistance, LayerMask.GetMask("Platform"));
                if (ray.collider != null)
                {
                    return;
                };
            }
            transform.Translate(moveDir, Space.World);
        }
    }
}