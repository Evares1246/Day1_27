using UnityEngine;
using Unity.Netcode;
using GameDesign.Utils;

namespace Game.Network
{
    public class ConnectionManager : Singleton<ConnectionManager>
    {
        [SerializeField] private string _gameSceneName = "02_Game";

        private void Start()
        {
            // 配置 ApprovalCallback 以禁止自动创建玩家对象
            // 我们将在场景加载完成后手动创建，确保 TableManager 已准备就绪
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            }
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            // 允许连接
            response.Approved = true;
            
            // 允许自动创建玩家对象 (默认行为)
            response.CreatePlayerObject = true;
            
            // 设置初始位置等 (可选)
            response.Position = Vector3.zero;
            response.Rotation = Quaternion.identity;
            
            Debug.Log($"[ConnectionManager] Client {request.ClientNetworkId} approved. Auto-Create Player: TRUE");
        }

        public void StartHost()
        {
            Debug.Log("[ConnectionManager] Starting Host...");
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("[ConnectionManager] Host started successfully.");
                LoadGameScene();
            }
            else
            {
                Debug.LogError("[ConnectionManager] Failed to start Host.");
            }
        }

        public void StartClient()
        {
            Debug.Log("[ConnectionManager] Starting Client...");
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("[ConnectionManager] Client started successfully.");
                // Client automatically loads scene synced by Host
            }
            else
            {
                Debug.LogError("[ConnectionManager] Failed to start Client.");
            }
        }

        private void LoadGameScene()
        {
            // Use NetworkSceneManager to switch scenes so clients follow
            NetworkManager.Singleton.SceneManager.LoadScene(_gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
