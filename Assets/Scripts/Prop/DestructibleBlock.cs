using UnityEngine;

namespace LudumDare57.Prop
{
    public class DestructibleBlock : MonoBehaviour, IDestructible
    {
        public int MoneyGained { set; get; }

        public GameObject GameObject => gameObject;

        public bool CanDestroy => true;

        private SpriteRenderer _sr;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        public void OnDestroy()
        { }

        public void ToggleHighlight(bool value)
        {
            _sr.color = value ? Color.red : Color.white;
        }
    }
}