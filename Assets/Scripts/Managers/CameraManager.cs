using Cinemachine;
using InGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : SingletonBehavior<CameraManager>
{
    [System.Serializable]
    private class BackgroundObject
    {
        public float speed;
        public SpriteRenderer spriteRenderer;
    }

    private class CameraShake
    {
        public float time;
        public float insensity;
        public float power;
    }

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
    private CinemachineBasicMultiChannelPerlin channelPerlin;
    private List<CameraShake> cameraShakes = new();

    [SerializeField] private List<BackgroundObject> backgroundObjects;

    [SerializeField] GameObject background;

    public Map NowMap => playerInMaps[0];
    private List<Map> playerInMaps = new List<Map>();

    protected override void OnCreated()
    {
        base.OnCreated();
        channelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

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

    public void Shake(float time, float insensity, float power)
    {
        cameraShakes.Add(new CameraShake()
        {
            time = time,
            insensity = insensity,
            power = power
        });
    }

    private void Update()
    {
        if (playerInMaps.Count <= 0) return;

        var size = (NowMap.mapCollider.bounds.center - GameManager.Instance.playerUnit.transform.position) * 3;
        background.transform.localPosition = Vector3.Lerp(background.transform.localPosition, new Vector3(size.x / NowMap.mapCollider.bounds.size.x, size.y / NowMap.mapCollider.bounds.size.y, 10), Time.deltaTime * 20);
        foreach (var obj in backgroundObjects)
        {
            obj.spriteRenderer.transform.Translate(Vector2.left * obj.speed * Time.deltaTime);
            if (Mathf.Abs(obj.spriteRenderer.transform.localPosition.x) > 20)
            {
                obj.spriteRenderer.transform.localPosition -= Mathf.Sign(obj.spriteRenderer.transform.localPosition.x) * 40 * Vector3.right;
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerInMaps.Count <= 0) return;

        UpdateCameraShake();
        NowMap.OnFixedUpdate();
    }

    private void UpdateCameraShake()
    {
        if (cameraShakes.Count < 0)
        {
            channelPerlin.m_AmplitudeGain = 0;
            channelPerlin.m_FrequencyGain = 0;
            return;
        }

        float power = 0;
        float insensity = 0;

        foreach(var cameraShake in cameraShakes)
        {
            cameraShake.time -= Time.fixedDeltaTime;
            if (cameraShake.time > 0)
            {
                power += cameraShake.power;
                insensity += cameraShake.insensity;
            }
        }

        cameraShakes.RemoveAll(x => x.time < 0);

        channelPerlin.m_AmplitudeGain = power;
        channelPerlin.m_FrequencyGain = insensity;
    }
}
