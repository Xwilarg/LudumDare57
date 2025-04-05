using UnityEngine;

namespace LudumDare57.Prop
{
    public interface IDestructible
    {
        public int MoneyGained { get; }
        public GameObject GameObject { get; }

        public void OnDestroy();
        public void ToggleHighlight(bool value);
    }
}