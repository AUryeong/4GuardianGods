using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private List<PolygonCollider2D> cameraBoundingShapes = new List<PolygonCollider2D>();

    public void Enqueue(PolygonCollider2D polygon)
    {
        cameraBoundingShapes.Add(polygon);
        if (cameraBoundingShapes.Count == 1)
            confiner2D.m_BoundingShape2D = cameraBoundingShapes[0];
    }

    public void DeQueue(PolygonCollider2D polygon)
    {
        int index = cameraBoundingShapes.IndexOf(polygon);
        if (index < 0) return;

        cameraBoundingShapes.RemoveAt(index);
        if (index == 0 && cameraBoundingShapes.Count > 0)
            confiner2D.m_BoundingShape2D = cameraBoundingShapes[0];
    }
}
