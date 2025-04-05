using System.Collections;
using UnityEngine;

namespace LudumDare57.Enemy
{
    public abstract class AEnemy : MonoBehaviour
    {
        protected Rigidbody2D _rb;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            StartCoroutine(Act());
        }

        public abstract void DoAction();

        private IEnumerator Act()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                DoAction();
            }
        }
    }
}