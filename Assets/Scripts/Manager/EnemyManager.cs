using LudumDare57.Enemy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LudumDare57.Manager
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { private set; get; }

        private readonly List<AEnemy> _enemies = new();

        private int AmountGood { set; get; }

        private void Awake()
        {
            Instance = this;
        }

        public void Register(AEnemy e)
        {
            _enemies.Add(e);
            if (!e.IsBad) AmountGood++;
        }

        public void Unregister(GameObject go)
        {
            _enemies.RemoveAll(x => x.gameObject.GetInstanceID() == go.GetInstanceID());
        }

        public bool AreBadEnemiesAlive => _enemies.Any(x => x.IsBad);
        public bool AreMostDead => _enemies.Count < AmountGood / 2f;
    }
}