using UnityEngine;
using Unity.Netcode;
using Game.Gameplay.TurnSystem;

namespace Game.Gameplay.Player
{
    /// <summary>
    /// 玩家控制器 (Player Seat)。
    /// 负责处理玩家的输入、视角切换 (第一人称/俯视)。
    /// 不直接在棋盘上移动，而是发送指令给 TurnManager。
    /// </summary>
    public class PlayerController : NetworkBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform _eyeStoneView; // 第一人称位置
        [SerializeField] private Transform _eagleView;    // 俯视位置
        
        // 本地状态
        private bool _isEagleView = false;
        private Camera _mainCamera;

        [Header("References")]
        [SerializeField] private GameDesign.Gameplay.InputReader _inputReader;

        [Header("Look Settings")]
        [SerializeField] private float _lookSensitivity = 0.5f;
        [SerializeField] private float _maxVerticalAngle = 60f;

        private float _currentPitch = 0f;
        private float _currentYaw = 0f;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            _mainCamera = Camera.main;
            
            // 初始化 InputReader
            if (_inputReader == null)
            {
                _inputReader = GetComponent<GameDesign.Gameplay.InputReader>();
                if (_inputReader == null)
                {
                    Debug.LogError("[PlayerController] InputReader component is missing!");
                }
            }

            if (_inputReader != null)
            {
                _inputReader.EnableInput();
                _inputReader.onToggleView += HandleToggleView;
                _inputReader.onClick += HandleClick;
            }

            // 初始化视角
            SwitchView(false);
            
            Debug.Log($"[PlayerController] Local Player Spawned: {OwnerClientId}");
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            if (_inputReader != null)
            {
                _inputReader.onToggleView -= HandleToggleView;
                _inputReader.onClick -= HandleClick;
            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            HandleLook();
        }

        private void HandleLook()
        {
            if (_inputReader == null || _isEagleView) return;

            Vector2 delta = _inputReader.LookDelta;
            if (delta == Vector2.zero) return;

            // 应用灵敏度
            float yawDelta = delta.x * _lookSensitivity;
            float pitchDelta = delta.y * _lookSensitivity;

            // 累加旋转
            _currentYaw += yawDelta;
            _currentPitch -= pitchDelta; // Y轴输入通常对应绕X轴负向旋转（抬头为负？）- 视InputSystem配置而定，通常鼠标上推是+Y，对应抬头是-Pitch

            // 限制垂直角度
            _currentPitch = Mathf.Clamp(_currentPitch, -_maxVerticalAngle, _maxVerticalAngle);

            // 应用旋转到 EyeStoneView (局部旋转，相对于座位)
            if (_eyeStoneView != null)
            {
                _eyeStoneView.localRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            }
        }

        private void HandleToggleView()
        {
            _isEagleView = !_isEagleView;
            SwitchView(_isEagleView);
        }

        private void HandleClick()
        {
            // 掷骰子输入
            // 只有在自己的回合且处于 RollDice 阶段时才有效
            TryRollDice();
        }

        private void SwitchView(bool isEagle)
        {
            if (_mainCamera == null) return;

            if (isEagle && _eagleView != null)
            {
                // 简单的位置切换，实际可以使用 DoTween 做平滑过渡
                _mainCamera.transform.position = _eagleView.position;
                _mainCamera.transform.rotation = _eagleView.rotation;
                _mainCamera.transform.SetParent(_eagleView);
            }
            else if (!isEagle && _eyeStoneView != null)
            {
                _mainCamera.transform.position = _eyeStoneView.position;
                _mainCamera.transform.rotation = _eyeStoneView.rotation;
                _mainCamera.transform.SetParent(_eyeStoneView);
            }
        }

        private void TryRollDice()
        {
            // 调用 TurnManager 的 RPC
            // 注意: 实际项目中最好不要直接引用单例的 RPC，而是通过 NetworkObject 发送
            // 这里为了演示方便，假设 TurnManager 是一个可以访问的 NetworkBehaviour
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.RollDiceServerRpc(OwnerClientId);
            }
        }
    }
}
