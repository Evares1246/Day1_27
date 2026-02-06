using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using GameDesign.Gameplay.TurnSystem;

namespace GameDesign.UI
{
    /// <summary>
    /// 处理局内顶层交互 UI。
    /// </summary>
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;

        private void Awake()
        {
            if (_startGameButton != null)
            {
                _startGameButton.onClick.AddListener(OnStartGameClicked);
                // 默认先隐藏，直到确认自己是 Server 且网络就绪
                _startGameButton.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            // --- 顶级开发者逻辑：动态感知网络状态 ---
            if (NetworkManager.Singleton == null) return;

            // 只有当我是 Server/Host，且游戏还没开始，且 TurnManager 已经在网络上生成了
            if (NetworkManager.Singleton.IsServer && TurnManager.Instance != null)
            {
                if (!TurnManager.Instance.IsGameStarted.Value && TurnManager.Instance.IsSpawned)
                {
                    if (!_startGameButton.gameObject.activeSelf)
                    {
                        _startGameButton.gameObject.SetActive(true);
                        Debug.Log("[UI] TurnManager ready, Start Button enabled.");
                    }
                }
                else
                {
                    if (_startGameButton.gameObject.activeSelf)
                        _startGameButton.gameObject.SetActive(false);
                }
            }
        }

        private void OnStartGameClicked()
        {
            if (TurnManager.Instance == null) return;

            // 关键修复：确保 TurnManager 已生成才调用 RPC
            if (TurnManager.Instance.IsSpawned)
            {
                Debug.Log("[UI] Requesting Game Start...");
                TurnManager.Instance.StartGameServerRpc();

                _startGameButton.interactable = false;
                _startGameButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("[UI] TurnManager not yet spawned on network. Please wait.");
            }
        }
    }
}