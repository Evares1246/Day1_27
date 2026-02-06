using UnityEngine;
using Unity.Netcode;
using Game.Core;

namespace Game.Gameplay.TurnSystem
{
    public enum TurnPhase
    {
        StartTurn,
        RollDice,
        Move,
        Event,
        EndTurn
    }

    public class TurnManager : NetworkBehaviour
    {
        public static TurnManager Instance { get; private set; }

        public NetworkVariable<int> CurrentPlayerIndex = new NetworkVariable<int>(0);
        public NetworkVariable<TurnPhase> CurrentPhase = new NetworkVariable<TurnPhase>(TurnPhase.StartTurn);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public override void OnNetworkSpawn()
        {
            // 不要自动开始
            // if (IsServer)
            // {
            //     StartTurn();
            // }
        }

        public override void OnNetworkDespawn()
        {
            // 清理代码 (如果有)
        }

        public void StartGame()
        {
            if (!IsServer) return;
            // 初始化玩家列表等
            StartTurn();
        }

        public void StartTurn()
        {
            if (!IsServer) return;
            CurrentPhase.Value = TurnPhase.StartTurn;
            Debug.Log($"[TurnManager] Player {CurrentPlayerIndex.Value} Turn Started");
            
            // Auto transition to Roll for now
            CurrentPhase.Value = TurnPhase.RollDice;
        }

        public void EndTurn()
        {
            if (!IsServer) return;
            
            // Move to next player
            // Assuming 4 players for now, need PlayerManager to get actual count
            int playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            if (playerCount == 0) playerCount = 1; // Fallback

            CurrentPlayerIndex.Value = (CurrentPlayerIndex.Value + 1) % playerCount;
            
            // Check for Day Cycle (if looped back to 0, or based on steps)
            // For now, simple round robin
            
            StartTurn();
        }

        // Called by UI or Player Input
        [ServerRpc(InvokePermission = RpcInvokePermission.Everyone)]
        public void RollDiceServerRpc(ulong clientId)
        {
            // TODO: 验证是否是当前玩家 (CurrentPlayerIndex 对应的 ClientId)
            
            int roll = Random.Range(1, 7);
            Debug.Log($"[TurnManager] Player {clientId} Rolled {roll}");
            
            CurrentPhase.Value = TurnPhase.Move;
            
            // 查找属于该 Client 的 Pawn 并移动
            // 这里假设我们有一个 PawnManager 或者简单的查找逻辑
            MovePlayerPawn(clientId, roll);
        }

        private async void MovePlayerPawn(ulong clientId, int steps)
        {
            // 简单的查找逻辑: 遍历所有 PawnController 找到 Owner 匹配的
            // 实际项目中应该缓存这个映射
            foreach (var pawn in FindObjectsByType<PawnController>(FindObjectsSortMode.None))
            {
                if (pawn.OwnerClientId.Value == clientId)
                {
                    await pawn.MoveStepsAsync(steps);
                    // 移动完成后，触发事件或结束回合
                    // EndTurn(); // 暂时自动结束回合用于测试
                    break;
                }
            }
        }
    }
}
