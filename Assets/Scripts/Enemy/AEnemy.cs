using LudumDare57.Manager;
using LudumDare57.Player;
using LudumDare57.Prop;
using System.Collections;
using UnityEngine;

namespace LudumDare57.Enemy
{
    public abstract class AEnemy : MonoBehaviour, IDestructible
    {
        protected Rigidbody2D _rb;

        protected PlayerController _target;

        [SerializeField]
        private Transform _eye;

        public int MoneyGained => Random.Range(20, 50);

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

                _target = PlayerManager.Instance.GetClosest(transform.position);
                if (_target != null && Vector2.Distance(transform.position, _target.transform.position) < 10f)
                {
                    DoAction();
                }
            }
        }

        private void Update()
        {
            if (_target == null)
            {
                _eye.transform.localPosition = Vector3.zero;
                return;
            }

            var dir = (_target.transform.position - transform.position).normalized;
            _eye.localPosition = dir * .25f;
        }

        public virtual void OnDestroy()
        { }
    }
}