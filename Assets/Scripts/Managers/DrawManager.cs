using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace InGame
{
    public class DrawManager : SingletonBehavior<DrawManager>
    {
        private Vector2 lastVector;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private EdgeCollider2D edgeCollider;
        [SerializeField] private SpriteRenderer lineSpriteRenderer;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D rigid;

        [SerializeField] private float distance = 0;
        private const float MAX_DISTANCE = 1000;

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var pos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                rigid.gameObject.layer = LayerMask.NameToLayer("Brush");
                rigid.bodyType = RigidbodyType2D.Dynamic;
                rigid.gravityScale = 1;
                rigid.velocity = (pos - GameManager.Instance.playerUnit.transform.position) * 2;

                Debug.Log(Mathf.Max(0, (MAX_DISTANCE-distance)/Mathf.Sqrt(edgeCollider.bounds.size.x * edgeCollider.bounds.size.y)));

                lineSpriteRenderer.gameObject.SetActive(true);
                lineSpriteRenderer.transform.localPosition = edgeCollider.bounds.center;
                lineSpriteRenderer.size = edgeCollider.bounds.size;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Time.timeScale = 1f;
                spriteRenderer.DOFade(0, 0.5f).SetUpdate(true);

                var vector = new List<Vector2>(lineRenderer.positionCount);
                for (int i = 0; i < lineRenderer.positionCount; i++)
                    vector.Add(lineRenderer.GetPosition(i));
                edgeCollider.points = vector.ToArray();
            }

            if (!Input.GetMouseButton(0)) return;
            Vector2 input = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                distance = MAX_DISTANCE;

                rigid.bodyType = RigidbodyType2D.Static;
                rigid.gameObject.layer = LayerMask.NameToLayer("Platform");
                
                lineRenderer.transform.position = Vector3.zero;
                lineRenderer.transform.rotation = quaternion.identity;

                Time.timeScale = 0.25f;
                spriteRenderer.DOFade(0.4f, 0.5f).SetUpdate(true);
                lineRenderer.positionCount = 1;
                var pos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(input);
                pos.z = 0;

                lineSpriteRenderer.gameObject.SetActive(false);
                lineRenderer.SetPosition(0, pos);
                lastVector = input;
            }

            if (distance <= 0) return;
            if (Vector2.Distance(lastVector, input) > 25)
            {
                float lastDistance = Vector2.Distance(lastVector, input);

                lastVector = input;
                var pos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(input);
                pos.z = 0;

                distance -= lastDistance;

                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
            }

            spriteRenderer.transform.position = GameManager.Instance.playerUnit.transform.position;
        }
    }
}