using Unity.Netcode;
using UnityEngine;
using Game.Gameplay; // For TableManager
using Cysharp.Threading.Tasks;

namespace Game.Gameplay.Player
{
    public class SeatAssigner : NetworkBehaviour
    {
        public override async void OnNetworkSpawn()
        {
            // 无论是不是 Owner 都要执行，确保每个客户端上的该玩家眼石都在正确座位
            // 使用 UniTask 等待 TableManager 初始化完成，防止 Race Condition
            await SyncSeatPositionAsync();
        }

        private async UniTask SyncSeatPositionAsync()
        {
            // 等待直到 TableManager.Instance 不为空
            // 设置一个超时时间防止死循环 (例如 5 秒)
            int timeoutMs = 5000;
            float startTime = Time.time;

            while (TableManager.Instance == null)
            {
                if (Time.time - startTime > timeoutMs / 1000f)
                {
                    Debug.LogError("[SeatAssigner] Timeout waiting for TableManager!");
                    return;
                }
                await UniTask.Yield(); // 等待下一帧
            }

            // 向场景中的 TableManager 请求对应的座位
            // 使用 OwnerClientId 确保每个玩家分配到唯一的位子
            if (TableManager.Instance != null)
            {
                // 注意：OwnerClientId 是 ulong，需要转换为 int
                Transform targetAnchor = TableManager.Instance.GetSeat((int)OwnerClientId);
                if (targetAnchor != null)
                {
                    transform.SetPositionAndRotation(targetAnchor.position, targetAnchor.rotation);
                    Debug.Log($"[SeatAssigner] Player {OwnerClientId} assigned to seat at {targetAnchor.position}");
                }
                else
                {
                    Debug.LogWarning($"[SeatAssigner] Player {OwnerClientId} could not find a seat!");
                }
            }
        }
    }
}
