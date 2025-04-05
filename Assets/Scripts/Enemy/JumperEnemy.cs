using LudumDare57.Manager;
using UnityEngine;

namespace LudumDare57.Enemy
{
    public class JumperEnemy : AEnemy
    {
        private const float MaxX = 10f;

        public override void DoAction()
        {
            var target = PlayerManager.Instance.GetClosest(transform.position);
            if (target == null)
            {
                Debug.LogWarning($"{name} didn't find any target");
                return;
            }

            var xDiff = target.transform.position.x - transform.position.x;
            var xDir = Mathf.Clamp(xDiff, -MaxX, MaxX);
            _rb.AddForce(new Vector2(xDir, 3f * MaxX / 4f).normalized * 30f, ForceMode2D.Impulse);
        }
    }
}