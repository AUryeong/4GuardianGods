using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class TempDrawer : MonoBehaviour
    {
        private Vector2 lastVector;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private EdgeCollider2D edgeCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;

        void Update()
        {
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
                Time.timeScale = 0.25f;
                spriteRenderer.DOFade(0.4f, 0.5f).SetUpdate(true);
                lineRenderer.positionCount = 1;
                var pos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(input);
                pos.z = 0;

                lineRenderer.SetPosition(0, pos);
                lastVector = input;
            }

            if (Vector2.Distance(lastVector, input) > 25)
            {
                lastVector = input;
                var pos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(input);
                pos.z = 0;

                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
            }

            spriteRenderer.transform.position = GameManager.Instance.transform.position;
        }
    }
}