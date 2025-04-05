using LudumDare57.SO;
using UnityEngine;

namespace LudumDare57.Manager
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField]
        private MapGenInfo _genInfo;

        [SerializeField]
        private GameObject _blockPrefab;

        private const float TileSize = 1.28f;

        private GameObject _mapContainer;

        private void Awake()
        {
            _mapContainer = new GameObject("Map");

            for (int x = -_genInfo.MapGenWidth; x <= _genInfo.MapGenWidth; x++)
            {
                var go = Instantiate(_blockPrefab, _mapContainer.transform);
                go.transform.position = new Vector2(x, -1f) * TileSize;
                go.tag = "Destructible";
            }
        }
    }
}