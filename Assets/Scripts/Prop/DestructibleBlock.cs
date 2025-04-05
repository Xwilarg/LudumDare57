using UnityEngine;

namespace LudumDare57.Prop
{
    public class DestructibleBlock : MonoBehaviour, IDestructible
    {
        public int MoneyGained { set; get; }

        public GameObject GameObject => gameObject;
    }
}