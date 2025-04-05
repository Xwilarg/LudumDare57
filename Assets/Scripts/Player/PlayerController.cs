using LudumDare57.Manager;
using LudumDare57.SO;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LudumDare57.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;
        public PlayerInfo Info => _info;

        [SerializeField]
        private TMP_Text _depthText;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private float _xMov;

        private bool _canJump = true;
        private float _hurtTimer;
        private Vector2 _hurtDirection;

        private Drill _drill;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _drill = GetComponent<Drill>();
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            PlayerManager.Instance.Register(this);
        }

        private void FixedUpdate()
        {
            if (_drill.IsDrilling) _rb.linearVelocity = _drill.DrilligDir * _info.DrillingSpeed;
            else if (_hurtTimer > 0f) _rb.linearVelocity = _hurtDirection * _info.HurtSpeed;
            else _rb.linearVelocity = new Vector2(_xMov * _info.Speed, _rb.linearVelocityY);
        }

        private void Update()
        {
            var depth = transform.position.y / 2f;

            var sign = depth >= 0f ? " " : "-";
            _depthText.text = $"Depth: {sign}{Mathf.Abs(depth):00 000}m";

            if (_hurtTimer > 0f)
            {
                _hurtTimer -= Time.deltaTime;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                StartCoroutine(PlayHurtEffect());
                _hurtTimer = _info.HurtDuration;
                _hurtDirection = (transform.position - collision.collider.transform.position).normalized;
            }
        }

        private IEnumerator PlayJumpCooldown() // Prevent player from spamming jump button to launch upward
        {
            _canJump = false;
            yield return new WaitForSeconds(_info.JumpCooldown);
            _canJump = true;
        }

        private IEnumerator PlayHurtEffect()
        {
            _sr.color = Color.red;
            yield return new WaitForSeconds(.1f);
            _sr.color = Color.white;
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

        private void OnDrawGizmos()
        {
            // Debug jump box
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position - Vector3.up, new Vector3(1f, .2f, 1f));
        }
    }
}