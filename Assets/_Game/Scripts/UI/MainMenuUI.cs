using UnityEngine;
using UnityEngine.UI;
using Game.Network;

namespace Game.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _quitButton;

        private void Start()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDestroy()
        {
            _hostButton.onClick.RemoveListener(OnHostClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
            _quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnHostClicked()
        {
            ConnectionManager.Instance.StartHost();
        }

        private void OnJoinClicked()
        {
            ConnectionManager.Instance.StartClient();
        }

        private void OnQuitClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
