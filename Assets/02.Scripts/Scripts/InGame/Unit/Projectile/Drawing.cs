using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public enum DrawingType
    {
        None,
        Drawing,
        Draw,
        Projectile,
        Remove
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
        [SerializeField] private UnitHit unitHit;
        public UnitHit UnitHit => unitHit;
        [SerializeField] private Rigidbody2D rigid;
        [SerializeField] private SpriteRenderer emitter;

        private float hitDuration;
        private const float HIT_DURATION = 0.1f;

        private List<Vector2> lines;
        private const float PROJECTILE_DURATION = 5;
        private const float DRAW_DURATION = 10;
        private const float REMOVE_DURATION = 1f;
        private float duration;
        public float Power { get; private set; }

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
            unitHit.SetHitAction(UpdateDuration);
            unitHit.SetDieAction(DieAction);
        }

        private void DieAction()
        {
            if (type == DrawingType.Remove)
                return;

            Type = DrawingType.Remove;
            DOTween.To(() => lineRenderer.endColor.a, x => lineRenderer.endColor = lineRenderer.endColor.GetChangeAlpha(x), 0, REMOVE_DURATION / 2).SetEase(Ease.InQuad);
        }

        private void UpdateDuration()
        {
            if (type == DrawingType.Remove)
                return;

            lineRenderer.material.SetInt("_Flash", 1);
            hitDuration = HIT_DURATION;
            Power = unitHit.Hp;
            duration = 0;
        }

        public void SetPower(float power)
        {
            Power = Mathf.Max(0, power / Mathf.Pow(edgeCollider.bounds.size.x * edgeCollider.bounds.size.y, 0.7f));
            unitHit.Hp = power;
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
            if (hitDuration > 0)
            {
                hitDuration -= deltaTime;
                if (hitDuration <= 0)
                {
                    lineRenderer.material.SetInt("_Flash", 0);
                }
            }

            switch (type)
            {
                case DrawingType.Remove:
                    duration += deltaTime;
                    SetPosition(lines.GetRange(0, (int)Mathf.Lerp(0, lines.Count, (REMOVE_DURATION - duration) / REMOVE_DURATION)));
                    lineRenderer.endColor = lineRenderer.endColor.GetChangeAlpha(Mathf.Lerp(0, 1, (REMOVE_DURATION - duration) / REMOVE_DURATION));
                    if (duration >= REMOVE_DURATION)
                    {
                        gameObject.SetActive(false);
                    }

                    break;
                case DrawingType.Draw:
                    duration += deltaTime;
                    if (rigid.velocity == Vector2.zero)
                    {
                        SetPosition(lines.GetRange(0, (int)Mathf.Lerp(0, lines.Count, (DRAW_DURATION - duration) / DRAW_DURATION * 4)));
                        lineRenderer.endColor = lineRenderer.endColor.GetChangeAlpha(Mathf.Lerp(0, 1, (DRAW_DURATION - duration) / DRAW_DURATION));
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
                        SetPosition(lines.GetRange(0, (int)Mathf.Lerp(0, lines.Count, (PROJECTILE_DURATION - duration) / PROJECTILE_DURATION * 2)));
                        lineRenderer.endColor = lineRenderer.endColor.GetChangeAlpha(Mathf.Lerp(0, 1, (PROJECTILE_DURATION - duration) / PROJECTILE_DURATION));
                        if (duration >= PROJECTILE_DURATION)
                        {
                            gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        unitCollider.UpdateCollider();
                    }

                    break;
            }
        }
    }
}