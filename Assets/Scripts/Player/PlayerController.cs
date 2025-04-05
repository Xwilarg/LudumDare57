using LudumDare57.Manager;
using LudumDare57.SO;
using System.Collections;
using System.Collections.Generic;
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

        [SerializeField]
        private RectTransform _healthContainer;

        [SerializeField]
        private GameObject _healthPrefab;
        private readonly List<GameObject> _lives = new();

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private float _xMov;

        private bool _canJump = true;
        private bool _isHurtCooldown;
        private float _hurtTimer;
        private Vector2 _hurtDirection;

        private int _maxLife;

        private Drill _drill;

        public bool IsOnExit { set; private get; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _drill = GetComponent<Drill>();
            _sr = GetComponent<SpriteRenderer>();

            _maxLife = _info.HealthCount;
        }

        private void Start()
        {
            ResetPlayer();
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
            if (collision.collider.CompareTag("Enemy") && !_isHurtCooldown)
            {
                AudioManager.Instance.PlayPlayerHit();
                StartCoroutine(PlayHurtEffect());
                _hurtTimer = _info.HurtDuration;
                _hurtDirection = (transform.position - collision.collider.transform.position).normalized;

                Destroy(_lives[0]);
                _lives.RemoveAt(0);
                if (_lives.Count == 0) ResetPlayer();
            }
        }

        public void UpgradeDrillSize()
        {
            _drill.UpgradeDrillSize();
        }

        public void UpgradeDrillCooldown()
        {
            _drill.UpgradeDrillCooldown();
        }

        private void ResetPlayer()
        {
            transform.position = Vector2.zero;
            _drill.ResetDrill();
            _hurtTimer = 0f;

            for (int c = 0; c < _healthContainer.childCount; c++) Destroy(_healthContainer.GetChild(c).gameObject);
            for (int i = 0; i < _maxLife; i++)
            {
                _lives.Add(Instantiate(_healthPrefab, _healthContainer));
            }
        }

        public void GainLife(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _maxLife++;
                _lives.Add(Instantiate(_healthPrefab, _healthContainer));
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
            _isHurtCooldown = true;
            yield return new WaitForSeconds(.1f);
            _sr.color = Color.white;
            _isHurtCooldown = false;
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

        public void OnInteract(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started && IsOnExit)
            {
                // TODO: Game end
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