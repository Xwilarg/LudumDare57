using LudumDare57.SO;
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

        private PlayerInfo _info;
        private Camera _cam;

        private Vector2? _drillingDir;
        private Color _drillBaseColor;
        private float _drillTimer;
        private bool _canDrill = true;

        public bool IsDrilling => _drillingDir != null;
        public Vector2 DrilligDir => _drillingDir.Value;

        private void Awake()
        {
            var pc = GetComponent<PlayerController>();
            _info = pc.Info;
            _cam = pc.PlayerCamera;

            _drillSr = _drill.GetComponent<SpriteRenderer>();
            _drillBaseColor = _drillSr.color;
        }

        private void Update()
        {
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

            if (_drillingDir == null) // Can't change direction while drilling
            {
                var worldMouse = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var dir = ((Vector2)(worldMouse - transform.position)).normalized;
                _drill.transform.up = dir;
                _drill.transform.position = transform.position + _drill.transform.up;
            }
        }

        public void OnDrill(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started && _canDrill)
            {
                var worldMouse = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                _drillingDir = ((Vector2)(worldMouse - transform.position)).normalized;

                _drillTimer = _info.DrillDuration;
                _canDrill = false;
            }
        }
    }
}