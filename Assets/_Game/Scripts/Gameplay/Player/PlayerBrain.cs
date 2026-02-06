using Unity.Netcode;
using UnityEngine;

namespace GameDesign.Gameplay.Player
{
    public class PlayerBrain : NetworkBehaviour
    {
        // 核心同步数据：棋子在轨道上的位置索引
        public NetworkVariable<int> PawnTileIndex = new NetworkVariable<int>(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [ServerRpc(InvokePermission = RpcInvokePermission.Everyone)]
        public void RequestPawnMoveServerRpc(int steps)
        {
            if (Map.BoardManager.Instance == null) return;

            int totalTiles = Map.BoardManager.Instance.GetTotalTileCount();
            PawnTileIndex.Value = (PawnTileIndex.Value + steps) % totalTiles;

            // 联动环境管理器的步数推进
            if (EnvironmentManager.Instance != null)
            {
                EnvironmentManager.Instance.AddGlobalStepsServerRpc(steps);
            }
        }
    }
}