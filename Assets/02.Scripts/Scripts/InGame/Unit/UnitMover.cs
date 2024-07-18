using System;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class UnitMover : MonoBehaviour
    {
        [Header("Value")]
        public bool IsPassPlatform;
        public bool IsPassOnlyPlatform;
        public Vector2 velocity;
        public float speed;
        public bool IsGround { get; private set; }
        [FormerlySerializedAs("horizontalLayCount")]
        [Range(2, 15)] public int layCount = 2;
        private Action<RaycastHit2D> colliderAction;

        [SerializeField] private Rigidbody2D rigid;
        public Rigidbody2D Rigid
        {
            get
            {
                if (!rigid)
                    rigid = GetComponent<Rigidbody2D>();
                return rigid;
            }
        }

        [SerializeField] private Collider2D unitCollider;
        public Collider2D Collider
        {
            get
            {
                if (!unitCollider)
                    unitCollider = GetComponent<Collider2D>();
                return unitCollider;
            }
        }

        public void SetColliderAction(Action<RaycastHit2D> action)
        {
            colliderAction = action;
        }

        public bool GetGround(float distance)
        {
            var moveDir = Vector2.down;
            var centerPivot = Collider.bounds.center;
            var size = Collider.bounds.size;

            float spacing = size.x / layCount;
            float rayCastDistance = Mathf.Abs(distance) + size.y / 2;

            for (int i = 0; i < layCount; i++)
            {
                Vector2 raycast = new Vector3(i * spacing - size.x / 2 + centerPivot.x + spacing, centerPivot.y);
                var ray = Physics2D.Raycast(raycast, moveDir, rayCastDistance, LayerMask.GetMask("Platform", "Brush"));
                if (ray.collider)
                {
                    return true;
                }
            }

            return false;
        }

        public void CheckGround()
        {
            IsGround = GetGround(1);
        }

        public bool Move(Vector2 moveValue, Action<RaycastHit2D> action = null)
        {
            var moveDir = moveValue.normalized;
            if (!IsPassPlatform)
            {
                var centerPivot = Collider.bounds.center;
                var size = Collider.bounds.size;

                float spacing = size.y / layCount;
                float rayCastDistance = Mathf.Abs(moveValue.x) + size.x / 2;

                for (int i = 0; i < layCount; i++)
                {
                    Vector2 raycast = new Vector3(centerPivot.x, i * spacing - size.y / 2 + centerPivot.y + spacing);
                    var ray = Physics2D.Raycast(raycast, moveDir, rayCastDistance, LayerMask.GetMask("Platform", "Brush"));
                    if (ray.collider)
                    {
                        if (!ray.collider.CompareTag("Brush") && IsPassOnlyPlatform)
                            continue;

                        if (action == null)
                            colliderAction?.Invoke(ray);
                        else
                            action?.Invoke(ray);
                        return false;
                    }
                }
            }

            transform.Translate(moveValue, Space.World);
            return true;
        }

        public void UpdateMove(float deltaTime)
        {
            if (velocity == Vector2.zero) return;

            Move(deltaTime * speed * velocity);
            velocity = Vector2.zero;
        }
    }
}