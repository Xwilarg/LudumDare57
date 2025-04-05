using LudumDare57.Manager;
using LudumDare57.Player;
using UnityEngine;

namespace LudumDare57.Enemy
{
    public class JumperEnemy : AEnemy
    {
        private const float MaxX = 10f;

        [SerializeField]
        private Transform _eye;

        private PlayerController _target;

        public override void DoAction()
        {
            _target = PlayerManager.Instance.GetClosest(transform.position);
            if (_target == null)
            {
                Debug.LogWarning($"{name} didn't find any target");
                return;
            }

            var xDiff = _target.transform.position.x - transform.position.x;
            var xDir = Mathf.Clamp(xDiff, -MaxX, MaxX);
            _rb.AddForce(new Vector2(xDir, 3f * MaxX / 4f).normalized * 30f, ForceMode2D.Impulse);
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
    }
}