using LudumDare57.Manager;
using LudumDare57.SO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LudumDare57.Player
{
    public class Drill : MonoBehaviour
    {
        [SerializeField]
        private GameObject _drill;
        private SpriteRenderer _drillSr;

        [SerializeField]
        private TriggerDetector _drillTrigger;

        [SerializeField]
        private GameObject _breakEffect;

        private PlayerInfo _info;

        private Vector2? _drillingDir;
        private Color _drillBaseColor;
        private float _drillTimer;
        private bool _canDrill = true;

        public bool IsDrilling => _drillingDir != null;
        public Vector2 DrilligDir => _drillingDir.Value;

        private readonly List<GameObject> _targetedBlocks = new();

        private string[] _drillables = new[] { "Destructible", "Enemy" };

        private void Awake()
        {
            var pc = GetComponent<PlayerController>();
            _info = pc.Info;

            _drillSr = _drill.GetComponent<SpriteRenderer>();
            _drillBaseColor = _drillSr.color;

            _drillTrigger.OnTriggerEnterEvt.AddListener((c) =>
            {
                var id = c.gameObject.GetInstanceID();
                if (_drillables.Any(x => c.CompareTag(x)) && !_targetedBlocks.Any(tb => tb.gameObject.GetInstanceID() == id))
                {
                    _targetedBlocks.Add(c.gameObject);
                }
            });
            _drillTrigger.OnTriggerExitEvt.AddListener((c) =>
            {
                var id = c.gameObject.GetInstanceID();
                _targetedBlocks.RemoveAll(bl => bl.gameObject.GetInstanceID() == id);
            });
        }

        private void Update()
        {
            // Update drill timer
            if (!_canDrill)
            {
                _drillTimer -= Time.deltaTime;

                // Update drill color depending of how long we have been drilling
                if (_drillingDir != null) _drillSr.color = Color.Lerp(_drillBaseColor, Color.red, 1f - (_drillTimer / _info.DrillDuration));
                else _drillSr.color = Color.Lerp(Color.red, _drillBaseColor, 1f - (_drillTimer / _info.DrillingCooldown));

                if (_drillTimer <= 0f)
                {
                    if (_drillingDir != null) // End of drill, start cooldown
                    {
                        _drillingDir = null;
                        _drillTimer = _info.DrillingCooldown;
                    }
                    else // End of cooldown, we can use the drill again
                    {
                        _canDrill = true;
                    }
                }
            }

            // Update drill object to follow mouse
            if (_drillingDir == null) // Can't change direction while drilling
            {
                var worldMouse = PlayerManager.Instance.Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var dir = ((Vector2)(worldMouse - transform.position)).normalized;
                _drill.transform.up = dir;
                _drill.transform.position = transform.position + _drill.transform.up;
            }

            // Update drill to destroy blocks in range
            if (_drillingDir != null)
            {
                for (int i = _targetedBlocks.Count - 1; i >= 0; i--)
                {
                    var bl = _targetedBlocks[i];
                    Destroy(Instantiate(_breakEffect, bl.transform.position, Quaternion.identity), .2f);
                    Destroy(bl);
                }
                _targetedBlocks.Clear();
            }
        }

        public void ResetDrill()
        {
            _drillTimer = 0f;
            _canDrill = true;
            _drillSr.color = _drillBaseColor;
        }

        public void OnDrill(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started && _canDrill)
            {
                var worldMouse = PlayerManager.Instance.Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                _drillingDir = ((Vector2)(worldMouse - transform.position)).normalized;

                _drillTimer = _info.DrillDuration;
                _canDrill = false;
            }
        }
    }
}