using LudumDare57.Prop;
using System.Collections;
using UnityEngine;

namespace LudumDare57.Enemy
{
    public abstract class AEnemy : MonoBehaviour, IDestructible
    {
        protected Rigidbody2D _rb;

        public int MoneyGained => Random.Range(50, 76);

        public GameObject GameObject => gameObject;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            StartCoroutine(Act());
        }

        public abstract void DoAction();

        private IEnumerator Act()
        {
            yield return new WaitForSeconds(Random.Range(0f, 2f));
            while (true)
            {
                yield return new WaitForSeconds(2f);
                DoAction();
            }
        }
    }
}