using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GameDesign.Gameplay.Map;

namespace GameDesign.Gameplay.Player
{
    public class PawnEntity : NetworkBehaviour
    {
        [Header("Binding Info")]
        public NetworkVariable<ulong> OwnerId = new NetworkVariable<ulong>(999);

        private PlayerBrain _boundBrain;
        private PawnVisualizer _visualizer;

        public override void OnNetworkSpawn()
        {
            _visualizer = GetComponent<PawnVisualizer>();
            WaitAndBind().Forget();
        }

        private async UniTaskVoid WaitAndBind()
        {
            // 顶级开发者习惯：使用异步等待而非 Coroutine，更易读且性能更好
            while (!NetworkManager.Singleton.ConnectedClients.ContainsKey(OwnerId.Value))
            {
                await UniTask.Delay(100);
            }

            var client = NetworkManager.Singleton.ConnectedClients[OwnerId.Value];
            if (client.PlayerObject != null)
            {
                _boundBrain = client.PlayerObject.GetComponent<PlayerBrain>();

                // 给 Visualizer 注入数据源
                if (_visualizer != null) _visualizer.Initialize(_boundBrain);

                // 初始对齐坐标
                transform.position = BoardManager.Instance.GetPosition(_boundBrain.PawnTileIndex.Value);
                Debug.Log($"[Pawn] Succesfully bound to Player {OwnerId.Value}");
            }
        }
    }
}