using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem; // 必须引入
using UnityEngine.EventSystems; // 用于 UI 检测
using DG.Tweening;
using GameDesign.Gameplay.TurnSystem;

namespace GameDesign.Gameplay.Player
{
    [RequireComponent(typeof(InputReader), typeof(PlayerBrain))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform _eyeStoneView;
        [SerializeField] private Transform _eagleView;

        [Header("Interaction Settings")]
        [SerializeField] private float _lookSensitivity = 0.2f;
        [SerializeField] private float _maxVerticalAngle = 60f;
        [SerializeField] private LayerMask _interactableLayer;

        private Camera _mainCamera;
        private InputReader _inputReader;
        private PlayerBrain _brain;

        private bool _isEagleView = false;
        private bool _isCursorUnlocked = false;

        private float _currentPitch = 0f;
        private float _currentYaw = 0f;

        public override void OnNetworkSpawn()
        {
            _brain = GetComponent<PlayerBrain>();
            if (!IsOwner) return;

            _mainCamera = Camera.main;
            _inputReader = GetComponent<InputReader>();

            if (_inputReader != null)
            {
                _inputReader.EnableInput();
                _inputReader.onToggleView += HandleToggleView;
                _inputReader.onClick += HandleInteraction;
                _inputReader.onUnlockCursor += ToggleCursorLock;
            }

            SwitchViewImmediate(false);
            RefreshCursorState();
        }

        private void Update()
        {
            if (!IsOwner) return;

            if (!_isEagleView && !_isCursorUnlocked)
            {
                HandleLookInput();
            }
        }

        private void HandleLookInput()
        {
            Vector2 lookDelta = _inputReader.LookDelta * _lookSensitivity;
            _currentYaw += lookDelta.x;
            _currentPitch = Mathf.Clamp(_currentPitch - lookDelta.y, -_maxVerticalAngle, _maxVerticalAngle);

            if (_eyeStoneView != null)
            {
                _eyeStoneView.localRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            }
        }

        private void HandleInteraction()
        {
            // --- 顶级开发者优化：UI 遮挡检测 ---
            // 如果鼠标已呼出，且当前点击在 UI 上，则直接截断逻辑，不发射射线
            if ((_isCursorUnlocked || _isEagleView) && EventSystem.current != null)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("[Interaction] Clicked on UI, ignoring game world raycast.");
                    return;
                }
            }

            // --- 修复点：使用 New Input System 获取鼠标位置 ---
            Vector3 mousePos = Mouse.current.position.ReadValue();

            // 如果光标是锁定的，从屏幕中心发射；如果解锁了，从鼠标位置发射
            Vector3 screenPoint = (_isCursorUnlocked || _isEagleView) ? mousePos : new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            Ray ray = _mainCamera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, 15f, _interactableLayer))
            {
                Debug.Log($"[Interaction] Hit Interactable: {hit.collider.name}");

                // 仅在自己回合且游戏开始时触发移动
                if (TurnManager.Instance != null && TurnManager.Instance.IsGameStarted.Value)
                {
                    if (TurnManager.Instance.ActivePlayerClientId.Value == OwnerClientId)
                    {
                        _brain.RequestMovePawnServerRpc(Random.Range(1, 7));
                    }
                }
            }
        }

        private void HandleToggleView()
        {
            _isEagleView = !_isEagleView;
            _isCursorUnlocked = false;
            RefreshCursorState();

            Transform target = _isEagleView ? _eagleView : _eyeStoneView;

            _mainCamera.transform.DOMove(target.position, 0.6f).SetEase(Ease.InOutCubic);
            _mainCamera.transform.DORotateQuaternion(target.rotation, 0.6f).SetEase(Ease.InOutCubic)
                .OnUpdate(() => {
                    _mainCamera.transform.SetParent(target);
                });
        }

        private void ToggleCursorLock()
        {
            if (_isEagleView) return;

            _isCursorUnlocked = !_isCursorUnlocked;
            RefreshCursorState();
        }

        private void RefreshCursorState()
        {
            bool shouldUnlock = _isEagleView || _isCursorUnlocked;
            Cursor.lockState = shouldUnlock ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = shouldUnlock;
        }

        private void SwitchViewImmediate(bool isEagle)
        {
            Transform target = isEagle ? _eagleView : _eyeStoneView;
            if (_mainCamera != null && target != null)
            {
                _mainCamera.transform.SetPositionAndRotation(target.position, target.rotation);
                _mainCamera.transform.SetParent(target);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            if (_inputReader != null)
            {
                _inputReader.onToggleView -= HandleToggleView;
                _inputReader.onClick -= HandleInteraction;
                _inputReader.onUnlockCursor -= ToggleCursorLock;
            }
        }
    }
}