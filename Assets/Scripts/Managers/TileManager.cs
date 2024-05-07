using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileDrawer<T>
{
    [OdinSerialize] public Dictionary<Color, T> tileDict = new();
    public Texture2D tileTexture;
}

public class TileManager : SingletonBehavior<TileManager>
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tileMap;

    [NonSerialized, OdinSerialize] private TileDrawer<TileBase> defaultTile;
    [NonSerialized, OdinSerialize] private TileDrawer<GameObject> backgroundTile;

    [SerializeField] private Texture2D splitTexture;

    [Button(nameof(CreateTile))]
    private void CreateTile()
    {
        foreach (var tilemap in grid.transform.GetComponentsInChildren<Tilemap>())
            DestroyImmediate(tilemap.gameObject);

        int width = defaultTile.tileTexture.width;
        int height = defaultTile.tileTexture.height;

        var vectors = new HashSet<Vector2Int>(width * height);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (vectors.Contains(new Vector2Int(j, i))) continue;

                List<TileData<TileBase>> tileDatas = new List<TileData<TileBase>>();
                List<TileData<GameObject>> backgroundDatas = new List<TileData<GameObject>>();
                int max = -1;
                for (int y = i; y < height; y++)
                {
                    for (int x = j; x < width; x++)
                    {
                        if (vectors.Contains(new Vector2Int(x, y))) continue;

                        vectors.Add(new Vector2Int(x, y));

                        var tileColor = defaultTile.tileTexture.GetPixel(x, y);
                        if (defaultTile.tileDict.TryGetValue(tileColor, out TileBase tileBase))
                            tileDatas.Add(new TileData<TileBase>(new Vector3Int(x, y), tileBase));

                        if (backgroundTile.tileDict.TryGetValue(backgroundTile.tileTexture.GetPixel(x,y), out GameObject backgroundData))
                            backgroundDatas.Add(new TileData<GameObject>(new Vector3Int(x, y), backgroundData));

                        if (max < 0)
                        {
                            var splitColor = splitTexture.GetPixel(x, y);
                            if (splitColor == Color.green)
                            {
                                for(int maxValue = x;maxValue < width; maxValue++)
                                {
                                    var color = splitTexture.GetPixel(maxValue, y);
                                    if (color != Color.green)
                                    {
                                        max = maxValue;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (max <= x)
                        {
                            break;
                        }
                    }
                    {
                        var splitColor = splitTexture.GetPixel(j, y);
                        if (splitColor == Color.green)
                            break;
                    }
                }
                if (tileDatas.Count <= 0 && backgroundDatas.Count <= 0) continue;

                var map = Instantiate(tileMap, grid.transform);
                map.ClearAllTiles();

                if (tileDatas.Count > 0)
                {
                    foreach (var tileData in tileDatas)
                        map.SetTile(tileData.pos, tileData.tileBase);
                    map.GetComponent<TileBaker>().Bake();
                }

                if (backgroundDatas.Count > 0)
                {
                    foreach (var tileData in backgroundDatas)
                    {
                        var obj = Instantiate(tileData.tileBase, map.transform);
                        obj.transform.position = tileData.pos;
                    }
                }
                continue;
            }
        }

    }
    public struct TileData<T>
    {
        public Vector3Int pos;
        public T tileBase;

        public TileData(Vector3Int pos, T tileBase)
        {
            this.pos = pos;
            this.tileBase = tileBase;
        }
    }
}
