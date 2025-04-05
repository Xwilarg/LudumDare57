using LudumDare57.Manager;
using UnityEngine;

namespace LudumDare57.Enemy
{
    public class JumperEnemy : AEnemy
    {
        public override void DoAction()
        {
            var target = PlayerManager.Instance.GetClosest(transform.position);
            if (target == null)
            {
                Debug.LogWarning($"{name} didn't find any target");
                return;
            }


        }
    }
}