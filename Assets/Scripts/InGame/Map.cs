using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public PolygonCollider2D mapCollider;

    private void Awake()
    {
        if(mapCollider == null)
            mapCollider = GetComponent<PolygonCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            CameraManager.Instance.Enqueue(this);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            CameraManager.Instance.DeQueue(this);
    }
}
