using Cinemachine;
using InGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundObject
{
    public float speed;
    public SpriteRenderer spriteRenderer;
}

public class CameraManager : SingletonBehavior<CameraManager>
{
    [SerializeField] private Camera mainCamera;
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            return mainCamera;
        }
    }

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineConfiner2D confiner2D;

    [SerializeField] private List<BackgroundObject> backgroundObjects;

    [SerializeField] GameObject background;

    public Map NowMap => playerInMaps[0];
    private List<Map> playerInMaps = new List<Map>();

    public void Enqueue(Map map)
    {
        playerInMaps.Add(map);
        if (playerInMaps.Count == 1)
            confiner2D.m_BoundingShape2D = playerInMaps[0].mapCollider;
    }

    public void DeQueue(Map map)
    {
        int index = playerInMaps.IndexOf(map);
        if (index < 0) return;

        playerInMaps.RemoveAt(index);
        if (index == 0 && playerInMaps.Count > 0)
            confiner2D.m_BoundingShape2D = playerInMaps[0].mapCollider;
    }

    private void FixedUpdate()
    {
        if (playerInMaps.Count <= 0) return;

        var size = (NowMap.mapCollider.bounds.center - GameManager.Instance.playerUnit.transform.position) * 3;
        background.transform.localPosition = Vector3.Lerp(background.transform.localPosition, new Vector3(size.x / NowMap.mapCollider.bounds.size.x, size.y/NowMap.mapCollider.bounds.size.y, 10), Time.deltaTime * 20);
        foreach (var obj in backgroundObjects)
        {
            obj.spriteRenderer.transform.Translate(Vector2.left * obj.speed * Time.deltaTime);
            if (Mathf.Abs(obj.spriteRenderer.transform.localPosition.x) > 20)
            {
                obj.spriteRenderer.transform.localPosition -= Mathf.Sign(obj.spriteRenderer.transform.localPosition.x) * 40 * Vector3.right;
            }
        }
    }
}
