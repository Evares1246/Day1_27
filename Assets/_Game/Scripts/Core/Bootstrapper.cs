using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private string _menuSceneName = "01_Menu";

        private async void Start()
        {
            Debug.Log("[Bootstrapper] 正在初始化游戏系统...");

            // 1. 初始化 GameManager
            var gm = GameManager.Instance;
            gm.ChangeState(GameState.Bootstrap);

            // 2. 模拟加载 (配置, 存档, 网络初始化)
            await InitializeServices();

            // 3. 加载菜单
            Debug.Log("[Bootstrapper] 正在加载菜单...");
            SceneManager.LoadScene(_menuSceneName);
        }

        private async UniTask InitializeServices()
        {
            // 这里可以放置繁重的初始化任务 (例如: 等待 Vivox/Relay 登录)
            await UniTask.Delay(1000); 
        }
    }
}
