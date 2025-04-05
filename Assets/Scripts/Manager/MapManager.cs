using LudumDare57.SO;
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

        private const float TileSize = 1.28f;
        private const int GameTopAreaY = -1;

        private GameObject _mapContainer;

        private void Awake()
        {
            _mapContainer = new GameObject("Map");

            for (int y = GameTopAreaY; y > -10; y--)
            {
                for (int x = -_genInfo.MapGenWidth; x <= _genInfo.MapGenWidth; x++)
                {
                    SpawnTile(x, y, destructible: Mathf.Abs(x) != _genInfo.MapGenWidth);
                }
            }
            SpawnWall(-_genInfo.MapGenWidth, 1f);
            SpawnWall(_genInfo.MapGenWidth, 1f);
        }

        private void SpawnWall(int x, float sizeX)
        {
            var wall = new GameObject("Wall", typeof(BoxCollider2D));
            wall.layer = LayerMask.NameToLayer("Map");
            wall.transform.position = new Vector2((x + (sizeX * (x < 0f ? -1f : 1f))) * TileSize, 0f);
            wall.transform.parent = _mapContainer.transform;
            var coll = wall.GetComponent<BoxCollider2D>();
            coll.size = new Vector2(sizeX * TileSize, 50f);
        }

        private void SpawnTile(int x, int y, bool destructible)
        {
            var isOnTop = y == GameTopAreaY;
            var availables = _tiles.Where(x => x.IsDestructible == destructible).OrderBy(x => isOnTop == x.IsOnTop ? 0 : 1);
            var first = availables.First();

            var go = Instantiate(_blockPrefab, _mapContainer.transform);
            go.transform.position = new Vector2(x, y) * TileSize;
            go.GetComponent<SpriteRenderer>().sprite = first.Sprites[Random.Range(0, first.Sprites.Length)];
            if (destructible) go.tag = "Destructible";
        }
    }
}