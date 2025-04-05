using LudumDare57.Prop;
using LudumDare57.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LudumDare57.Manager
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField]
        private MapGenInfo _genInfo;

        [SerializeField]
        private TileInfo[] _tiles;

        [SerializeField]
        private GameObject _blockPrefab;

        [SerializeField]
        private BoxCollider2D _genBoundingBox;

        [SerializeField]
        private GameObject[] _enemyPrefabs;

        private GameObject _mapContainer;

        private const float TileSize = 1.28f;
        private const int GameTopAreaY = -1;

        private void Awake()
        {
            _mapContainer = new GameObject("Map");
            _genBoundingBox.size = new Vector2((_genInfo.MapGenWidth * 2f + 1f) * TileSize, _genBoundingBox.size.y);

            for (int area = 0; area < 5; area++)
            {
                GenerateArea(area);
            }
            SpawnWall(-_genInfo.MapGenWidth, 1f);
            SpawnWall(_genInfo.MapGenWidth, 1f);
        }

        private void GenerateArea(int yOffset)
        {
            for (int y = 0; y < _genInfo.AreaHeight; y++)
            {
                for (int x = -_genInfo.MapGenWidth; x <= _genInfo.MapGenWidth; x++)
                {
                    SpawnTile(
                        x: x,
                        y: GameTopAreaY - (yOffset * (_genInfo.AreaHeight + _genInfo.AreaInterSpacing)) - y,
                        destructible: (y != 0 || x != 0) && Mathf.Abs(x) != _genInfo.MapGenWidth
                    );
                }
            }
            for (int y = 0; y < _genInfo.AreaInterSpacing; y++)
            {
                SpawnTile(
                    x: -_genInfo.MapGenWidth,
                    y: GameTopAreaY - (yOffset * (_genInfo.AreaHeight + _genInfo.AreaInterSpacing)) - _genInfo.AreaHeight - y,
                    destructible: false
                );
                SpawnTile(
                    x: _genInfo.MapGenWidth,
                    y: GameTopAreaY - (yOffset * (_genInfo.AreaHeight + _genInfo.AreaInterSpacing)) - _genInfo.AreaHeight - y,
                    destructible: false
                );
            }
            var enemyCount = Random.Range(_genInfo.EnemyPerArea.Min, _genInfo.EnemyPerArea.Max + 1);
            for (int i = 0; i < enemyCount; i++)
            {
                var spawnX = Random.Range(-_genInfo.MapGenWidth + 1, _genInfo.MapGenWidth - 1) * TileSize;
                var spawnY = Random.Range(
                    GameTopAreaY - (yOffset * (_genInfo.AreaHeight + _genInfo.AreaInterSpacing)) - _genInfo.AreaHeight - (_genInfo.AreaInterSpacing - 1f),
                    GameTopAreaY - (yOffset * (_genInfo.AreaHeight + _genInfo.AreaInterSpacing)) - _genInfo.AreaHeight - (_genInfo.AreaInterSpacing / 2f)
                ) * TileSize;
                Instantiate(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)], new Vector2(spawnX, spawnY), Quaternion.identity);
            }
        }

        private void SpawnWall(int x, float sizeX)
        {
            var wall = new GameObject("Wall", typeof(BoxCollider2D))
            {
                layer = LayerMask.NameToLayer("Map")
            };
            wall.transform.position = new Vector2((x + (sizeX * (x < 0f ? -1f : 1f))) * TileSize, 0f);
            wall.transform.parent = _mapContainer.transform;
            var coll = wall.GetComponent<BoxCollider2D>();
            coll.size = new Vector2(sizeX * TileSize, 50f);
        }

        private void SpawnTile(int x, int y, bool destructible)
        {
            var isOnTop = y == GameTopAreaY;
            IEnumerable<TileInfo> availables = _tiles.Where(x => x.IsDestructible == destructible).OrderBy(x => isOnTop == x.IsOnTop ? 0 : 1);

            var go = Instantiate(_blockPrefab, _mapContainer.transform);
            go.transform.position = new Vector2(x, y) * TileSize;
            if (destructible)
            {
                var bl = go.AddComponent<DestructibleBlock>();
                if (isOnTop)
                {
                    bl.MoneyGained = Random.Range(0, 2);
                }
                else
                {
                    var lucky = Random.Range(0, 100) < _genInfo.GoldChance;
                    if (lucky)
                    {
                        availables = _tiles.Where(x => x.IsValuable);
                        bl.MoneyGained = Random.Range(10, 30);
                    }
                    else bl.MoneyGained = Random.Range(0, 2);
                }
            }
            var first = availables.First();
            go.GetComponent<SpriteRenderer>().sprite = first.Sprites[Random.Range(0, first.Sprites.Length)];
        }
    }
}