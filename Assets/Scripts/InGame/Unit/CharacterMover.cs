using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class CharacterMover : MonoBehaviour
{
    public float speed;
    [Range(2, 15)]
    public int horizontalLayCount;

    public Rigidbody2D Rigid
    {
        get
        {
            return rigid ??= GetComponent<Rigidbody2D>();
        }
    }
    private Rigidbody2D rigid;
    
    public BoxCollider2D BoxCollider2D
    {
        get
        {
            return boxCollider2D ??= GetComponent<BoxCollider2D>();
        }
    }
    
    private BoxCollider2D boxCollider2D;

    public void Move(float speedMultiplier = 1)
    {
        if (speedMultiplier == 0) return;

        var moveDir = new Vector2(speedMultiplier * Time.deltaTime * speed, 0);

        var centerPivot = boxCollider2D.offset;
        var size = boxCollider2D.size;

        float face = Mathf.Sign(speedMultiplier);
        float faceReverse = (speedMultiplier > 0) ? -1 : 1;
        float spacing = size.y * 2 / (horizontalLayCount - 1);
        float rayCastDistance = Mathf.Abs(moveDir.x) + size.x; 

        for (int i = 0; i < horizontalLayCount; i++)
        {
            Vector2 raycast = transform.position + new Vector3(centerPivot.x * faceReverse, i * spacing - size.y + centerPivot.y);
            RaycastHit2D ray = Physics2D.Raycast(raycast, face * Vector2.right, rayCastDistance, LayerMask.GetMask("Platform"));
            if (ray.collider != null) return;
        }
        transform.Translate(moveDir);
    }
}
