using Unity.Netcode;
using UnityEngine;
using GameDesign.Gameplay.Map;

namespace GameDesign.Gameplay.Player
{
    public class PlayerBrain : NetworkBehaviour
    {
        [Header("Synced Stats")]
        public NetworkVariable<int> Gold = new NetworkVariable<int>(200);
        public NetworkVariable<int> PawnTileIndex = new NetworkVariable<int>(0);

        [ServerRpc(InvokePermission = RpcInvokePermission.Everyone)]
        public void RequestMovePawnServerRpc(int steps)
        {
            if (!IsServer) return;

            int total = BoardManager.Instance.TotalTiles;
            int nextIndex = (PawnTileIndex.Value + steps) % total;
            PawnTileIndex.Value = nextIndex;

            // 推进全局步数（用于昼夜季节切换）
            if (EnvironmentManager.Instance != null)
            {
                EnvironmentManager.Instance.AddGlobalStepsServerRpc(steps);
            }

            Debug.Log($"[Server] Player {OwnerClientId} logic position: {nextIndex}");
        }
    }
}