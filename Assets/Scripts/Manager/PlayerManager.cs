using LudumDare57.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LudumDare57.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { private set; get; }

        private readonly List<PlayerController> _players = new();

        private void Awake()
        {
            Instance = this;
        }

        public void Register(PlayerController pc)
        {
            _players.Add(pc);
        }

        public PlayerController GetClosest(Vector2 target)
            => _players.OrderBy(p => Mathf.Pow(target.x - p.transform.position.x, 2) + Mathf.Pow(target.y - p.transform.position.y, 2)).FirstOrDefault();
    }
}