using UnityEngine;

namespace LudumDare57.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/MapGenInfo", fileName = "MapGenInfo")]
    public class MapGenInfo : ScriptableObject
    {
        public int MapGenWidth;
        public int AreaCount;
        public int AreaHeight;
        public int AreaInterSpacing;

        public Range<int> EnemyPerArea;
        public Range<float> EnemyReactionTime;
        public float GoldChance;
    }

    [System.Serializable]
    public record Range<T>
    {
        public T Min;
        public T Max;
    }

}