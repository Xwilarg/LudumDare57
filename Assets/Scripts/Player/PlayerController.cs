using LudumDare57.SO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LudumDare57
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;

        [SerializeField]
        private GameObject _drill;

        [SerializeField]
        private Camera _cam;

        private Rigidbody2D _rb;
        private float _xMov;

        private bool _canJump = true;
        public bool _canDrill = true;

        private Vector2? _drillingDir;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (_drillingDir != null) return; // Can't change drill pos while drilling

            // Drill follow mouse
            var worldMouse = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var dir = ((Vector2)(worldMouse - transform.position)).normalized;
            _drill.transform.up = dir;
            _drill.transform.position = transform.position + _drill.transform.up;
        }

        private void FixedUpdate()
        {
            if (_drillingDir == null) _rb.linearVelocity = new Vector2(_xMov * _info.Speed, _rb.linearVelocityY);
            else _rb.linearVelocity = _drillingDir.Value * _info.DrillingSpeed;
        }

        private IEnumerator PlayJumpCooldown() // Prevent player from spamming jump button to launch upward
        {
            _canJump = false;
            yield return new WaitForSeconds(_info.JumpCooldown);
            _canJump = true;
        }

        private IEnumerator PlayDrillCooldown()
        {
            _canDrill = false;
            yield return new WaitForSeconds(_info.DrillDuration);
            _drillingDir = null;
            yield return new WaitForSeconds(_info.DrillingCooldown);
            _canDrill = true;
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            _xMov = value.ReadValue<Vector2>().x;
            if (_xMov < 0f) _xMov = -1f;
            else if (_xMov > 0f) _xMov = 1f;
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started && _canJump)
            {
                // Check for floor under player
                var under = Physics2D.OverlapBox((Vector2)transform.position - Vector2.up, new Vector2(1f, .2f), 0f, LayerMask.GetMask("Map"));
                if (under != null)
                {
                    _rb.AddForce(Vector2.up * _info.JumpForce, ForceMode2D.Impulse);
                    StartCoroutine(PlayJumpCooldown());
                }
            }
        }

        public void OnDrill(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started && _canDrill)
            {
                var worldMouse = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                _drillingDir = ((Vector2)(worldMouse - transform.position)).normalized;
                StartCoroutine(PlayDrillCooldown());
            }
        }

        private void OnDrawGizmos()
        {
            // Debug jump box
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position - Vector3.up, new Vector3(1f, .2f, 1f));
        }
    }
}