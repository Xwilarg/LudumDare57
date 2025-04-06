using LudumDare57.Enemy;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare57.Manager
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { private set; get; }

        private readonly List<AEnemy> _enemies = new();

        public void Register(AEnemy e)
        {
            _enemies.Add(e);
        }

        public void Unregister(GameObject go)
        {
            _enemies.RemoveAll(x => x.gameObject.GetInstanceID() == go.GetInstanceID());
        }
    }
}