using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public enum DrawingType
    {
        None,
        Drawing,
        Draw,
        Projectile
    }

    public class Drawing : MonoBehaviour
    {
        public DrawingType Type
        {
            set
            {
                switch (value)
                {
                    case DrawingType.Drawing:
                        rigid.bodyType = RigidbodyType2D.Static;
                        gameObject.layer = LayerMask.NameToLayer("DrawingBrush");
                        break;
                    case DrawingType.Draw:
                        gameObject.layer = LayerMask.NameToLayer("Brush");
                        duration = 0;
                        break;
                    case DrawingType.Projectile:
                        rigid.bodyType = RigidbodyType2D.Dynamic;
                        gameObject.layer = LayerMask.NameToLayer("Projectile");
                        duration = 0;
                        break;
                    default:
                    case DrawingType.None:
                        break;
                }

                type = value;
            }
            get { return type; }
        }
        private DrawingType type = DrawingType.None;

        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private UnitCollider unitCollider;
        [SerializeField] private Rigidbody2D rigid;
        [SerializeField] private SpriteRenderer emitter;

        private List<Vector2> lines;
        private const float PROJECTILE_DURATION = 5;
        private const float DRAW_DURATION = 10;
        private float duration;
        public float Power;

        private EdgeCollider2D EdgeCollider
        {
            get
            {
                if (edgeCollider == null)
                    edgeCollider = unitCollider.Collider as EdgeCollider2D;
                return edgeCollider;
            }
        }
        private EdgeCollider2D edgeCollider;

        private void Awake()
        {
            unitCollider.SetColliderAction(CheckCollider);
        }

        public void SetPower(float power)
        {
            Power = Mathf.Max(0, power / Mathf.Sqrt(edgeCollider.bounds.size.x * edgeCollider.bounds.size.y));
        }

        public void Throw(Vector2 vector)
        {
            rigid.velocity = Power * 3 * vector;
            
            CameraManager.Instance.Shake(0.3f, Power / 5, Power / 2);
        }

        private void CheckCollider(List<Collider2D> colliders)
        {
            foreach (var enemyColldier in colliders)
            {
                enemyColldier.GetComponent<Enemy>().UnitHit.Hit(Power);
            }
        }

        public void SetPosition(List<Vector2> linePosition)
        {
            lineRenderer.positionCount = linePosition.Count;
            for (int i = 0; i < linePosition.Count; i++)
                lineRenderer.SetPosition(i, linePosition[i]);
            EdgeCollider.SetPoints(linePosition);

            lines = linePosition;

            emitter.transform.localPosition = edgeCollider.bounds.center;
            emitter.size = edgeCollider.bounds.size;
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            switch (type)
            {
                case DrawingType.Draw:
                    duration += deltaTime;
                    if (rigid.velocity == Vector2.zero)
                    {
                        SetPosition(lines.GetRange(0, (int)Mathf.Lerp(0, lines.Count, (DRAW_DURATION - duration) / DRAW_DURATION * 4)));
                        if (duration >= DRAW_DURATION)
                        {
                            gameObject.SetActive(false);
                        }
                    }

                    break;
                case DrawingType.Projectile:
                    duration += deltaTime;
                    if (rigid.velocity == Vector2.zero)
                    {
                        SetPosition(lines.GetRange(0, (int)Mathf.Lerp(0, lines.Count, (DRAW_DURATION - duration) / PROJECTILE_DURATION * 2)));
                        if (duration >= PROJECTILE_DURATION)
                        {
                            gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        unitCollider.UpdateCollider(deltaTime);
                    }

                    break;
            }
        }
    }
}