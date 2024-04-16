using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : SingletonBehavior<TileManager>
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tileMap;

    [SerializeField] private Dictionary<Color, TileBase> tileDict = new();

    public Texture2D tileTexture;
    public Texture2D splitTexture;

    [Button(nameof(CreateTile))]
    private void CreateTile()
    {
        foreach (var tilemap in grid.transform.GetComponentsInChildren<Tilemap>())
            DestroyImmediate(tilemap.gameObject);

        int width = tileTexture.width;
        int height = tileTexture.height;

        var vectors = new HashSet<Vector2Int>(width * height);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (vectors.Contains(new Vector2Int(j, i))) continue;

                List<TileData> tileDatas = new List<TileData>();
                int max = -1;
                for (int y = i; y < height; y++)
                {
                    for (int x = j; x < width; x++)
                    {
                        if (vectors.Contains(new Vector2Int(x, y))) continue;

                        vectors.Add(new Vector2Int(x, y));

                        var tileColor = tileTexture.GetPixel(x, y);
                        if (tileDict.TryGetValue(tileColor, out TileBase tileBase))
                            tileDatas.Add(new TileData(new Vector3Int(x, y), tileBase));

                        if (max < 0)
                        {
                            var splitcolor = splitTexture.GetPixel(x, y);
                            if (splitcolor == Color.green)
                            {
                                max = x;
                                break;
                            }
                        }
                        else if (max <= x)
                        {
                            break;
                        }
                    }
                    {
                        var splitcolor = splitTexture.GetPixel(j, y);
                        if (splitcolor == Color.green)
                            break;
                    }
                }
                if (tileDatas.Count <= 0) continue;

                var map = Instantiate(tileMap, grid.transform);
                map.ClearAllTiles();
                foreach (var tileData in tileDatas)
                    map.SetTile(tileData.pos, tileData.tileBase);
                map.GetComponent<TileBaker>().Bake();
            }
        }

    }
    public struct TileData
    {
        public Vector3Int pos;
        public TileBase tileBase;

        public TileData(Vector3Int pos, TileBase tileBase)
        {
            this.pos = pos;
            this.tileBase = tileBase;
        }
    }
}
