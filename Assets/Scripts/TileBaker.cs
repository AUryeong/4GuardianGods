using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBaker : MonoBehaviour
{
    [ShowInInspector] public Bounds Bounds { get; private set; }

    [Button("Bake")]
    private void Bake()
    {
        foreach (var polygon in gameObject.GetComponentsInChildren<PolygonCollider2D>())
        {
            if (polygon != null && polygon.gameObject != gameObject)
                DestroyImmediate(polygon.gameObject);
        }

        var colliderObj = new GameObject("Collider")
        {
            transform =
            {
                parent = transform,
                localPosition = Vector2.zero
            },
            layer = 6
        };
        var childPolygon = colliderObj.AddComponent<PolygonCollider2D>();

        var tileMap = gameObject.GetComponent<Tilemap>();

        var tileMapCollider = gameObject.AddComponent<TilemapCollider2D>();
        var rigid = gameObject.AddComponent<Rigidbody2D>();
        var compositeCollider2D = gameObject.AddComponent<CompositeCollider2D>();

        tileMap.RefreshAllTiles();
        tileMap.CompressBounds();

        rigid.bodyType = RigidbodyType2D.Static;

        tileMapCollider.usedByComposite = true;

        compositeCollider2D.generationType = CompositeCollider2D.GenerationType.Manual;
        compositeCollider2D.GenerateGeometry();

        Bounds = compositeCollider2D.bounds;

        var points = new List<Vector2>();
        childPolygon.pathCount = compositeCollider2D.shapeCount;

        for (int i = 0; i < compositeCollider2D.shapeCount; i++)
        {
            int path = compositeCollider2D.GetPath(i, points);
            childPolygon.SetPath(i, points.Take(path).ToArray());
        }

        var polygonPoints = new Vector2[4]
        {
            new Vector2(Bounds.min.x, Bounds.min.y),
            new Vector2(Bounds.min.x, Bounds.max.y),
            new Vector2(Bounds.max.x, Bounds.max.y),
            new Vector2(Bounds.max.x, Bounds.min.y)
        };

        var thisPolygon = gameObject.transform.parent.GetOrAddComponent<PolygonCollider2D>();
        thisPolygon.isTrigger = true;
        thisPolygon.points = polygonPoints;

        DestroyImmediate(compositeCollider2D);
        DestroyImmediate(rigid);
        DestroyImmediate(tileMapCollider);
    }
}