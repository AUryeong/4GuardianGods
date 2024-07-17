using Cinemachine;
using InGame;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraManager : SingletonBehavior<CameraManager>
{
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

    public Map NowMap => playerInMaps.Count <= 0 ? null : playerInMaps[0];
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
        {
            confiner2D.m_BoundingShape2D = playerInMaps[0].mapCollider;
            playerInMaps[0].OnEnter();
        }
    }

    public void DeQueue(Map map)
    {
        int index = playerInMaps.IndexOf(map);
        if (index < 0) return;

        playerInMaps.RemoveAt(index);
        map.OnExit();
        if (index == 0 && playerInMaps.Count > 0)
        {
            confiner2D.m_BoundingShape2D = playerInMaps[0].mapCollider;
            playerInMaps[0].OnEnter();
        }
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

    public void SetOrthographic(float size)
    {
        DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, size, 1f).SetEase(Ease.OutQuad).SetUpdate(true);
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

        foreach (var cameraShake in cameraShakes)
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