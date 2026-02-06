using Unity.Netcode;
using UnityEngine;
using GameDesign.Utils;

namespace GameDesign.Gameplay.TurnSystem
{
    /// <summary>
    /// 全局回合管理器：控制游戏整体进度和玩家轮转。
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class TurnManager : Singleton_N<TurnManager>
    {
        [Header("Sync States")]
        public NetworkVariable<bool> IsGameStarted = new NetworkVariable<bool>(false);
        public NetworkVariable<ulong> ActivePlayerClientId = new NetworkVariable<ulong>(0);
        public NetworkVariable<int> TurnCount = new NetworkVariable<int>(1);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"[TurnManager] Network Spawned. Role: {(IsServer ? "Server/Host" : "Client")}");
        }

        /// <summary>
        /// 供 UI 调用：通过服务器正式开启赌局
        /// </summary>
        [ServerRpc(InvokePermission = RpcInvokePermission.Everyone)]
        public void StartGameServerRpc()
        {
            // 顶级开发者习惯：双重验证
            if (!IsServer) return;
            if (IsGameStarted.Value) return;

            IsGameStarted.Value = true;

            // 默认设置第一个连入的玩家（通常是 Host）为起始玩家
            if (NetworkManager.Singleton.ConnectedClientsIds.Count > 0)
            {
                ActivePlayerClientId.Value = NetworkManager.Singleton.ConnectedClientsIds[0];
            }

            Debug.Log("<color=green>[TurnManager] The soul gambling has officially begun!</color>");
        }

        [ServerRpc(InvokePermission = RpcInvokePermission.Everyone)]
        public void RollDiceServerRpc(ulong clientId)
        {
            if (!IsServer || !IsGameStarted.Value) return;
            if (clientId != ActivePlayerClientId.Value) return;

            Debug.Log($"[Server] Player {clientId} is authorized to roll.");
        }
    }
}