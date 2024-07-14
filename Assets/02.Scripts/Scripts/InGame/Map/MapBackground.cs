using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class MapBackground : MonoBehaviour
    {
        [System.Serializable]
        private class BackgroundObject
        {
            public SpriteRenderer spriteRenderer;
            public float speed;
        }

        [SerializeField] private List<BackgroundObject> backgroundObjects;

        private const float BACKGROUND_DISTANCE = 20;
        private const float BACKGROUND_SIZE = 3;
        private const float BACKGROUND_FOLLOW_SPEED = 20;

        private void Start()
        {
            transform.SetParent(CameraManager.Instance.MainCamera.transform);
        }

        private void Update()
        {
            var playerInMap = CameraManager.Instance.NowMap;
            if (!playerInMap) return;

            var bounds = playerInMap.mapCollider.bounds;
            var size = (bounds.center - GameManager.Instance.playerUnit.transform.position) * BACKGROUND_SIZE;

            transform.position = Vector3.Lerp(transform.position,
                CameraManager.Instance.MainCamera.transform.position + 
                 new Vector3(size.x / bounds.size.x, size.y / bounds.size.y, 10), Time.deltaTime * BACKGROUND_FOLLOW_SPEED);
            foreach (var obj in backgroundObjects)
            {
                obj.spriteRenderer.transform.Translate(obj.speed * Time.deltaTime * Vector3.left);
                if (Mathf.Abs(obj.spriteRenderer.transform.localPosition.x) > BACKGROUND_DISTANCE)
                {
                    obj.spriteRenderer.transform.localPosition -= Mathf.Sign(obj.spriteRenderer.transform.localPosition.x) * BACKGROUND_DISTANCE * 2 * Vector3.right;
                }
            }
        }
    }
}