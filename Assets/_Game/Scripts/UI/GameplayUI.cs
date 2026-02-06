using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Game.Gameplay.TurnSystem;

namespace Game.UI
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;

        private void Start()
        {
            _startGameButton.onClick.AddListener(OnStartGameClicked);
            
            // 只有 Host 才能看到开始游戏按钮
            if (!NetworkManager.Singleton.IsServer)
            {
                _startGameButton.gameObject.SetActive(false);
            }
        }

        private void OnStartGameClicked()
        {
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.StartGame();
                _startGameButton.interactable = false; // 防止重复点击
            }
        }
    }
}
