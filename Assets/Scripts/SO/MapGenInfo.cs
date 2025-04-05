using UnityEngine;

namespace LudumDare57.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/MapGenInfo", fileName = "MapGenInfo")]
    public class MapGenInfo : ScriptableObject
    {
        public int MapGenWidth;
        public int AreaHeight;
        public int AreaInterSpacing;
    }
}