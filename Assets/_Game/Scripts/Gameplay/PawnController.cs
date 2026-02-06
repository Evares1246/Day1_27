using UnityEngine;
using Unity.Netcode;
using GameDesign.Gameplay.Map;
using Cysharp.Threading.Tasks;

namespace Game.Gameplay
{
    /// <summary>
    /// 代表棋盘上的棋子 (Pawn)。
    /// 仅由服务器控制移动，客户端同步位置。
    /// </summary>
    public class PawnController : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpHeight = 0.5f;

        // 棋子当前所在的格子索引
        public NetworkVariable<int> CurrentTileIndex = new NetworkVariable<int>(0);
        
        // 归属的玩家ID (ClientID)
        public NetworkVariable<ulong> OwnerClientId = new NetworkVariable<ulong>();

        public override void OnNetworkSpawn()
        {
            // 初始化位置
            if (BoardManager.Instance != null)
            {
                SnapToTile(CurrentTileIndex.Value);
            }
            
            CurrentTileIndex.OnValueChanged += OnTileIndexChanged;
        }

        public override void OnNetworkDespawn()
        {
            CurrentTileIndex.OnValueChanged -= OnTileIndexChanged;
        }

        private void OnTileIndexChanged(int oldIndex, int newIndex)
        {
            // 客户端位置平滑同步可以放在这里，或者使用 NetworkTransform
            // 这里为了简单，直接瞬移或触发移动动画
            if (!IsServer) 
            {
                // 客户端可以在这里播放移动动画
                SnapToTile(newIndex); 
            }
        }

        /// <summary>
        /// 瞬间移动到指定格子
        /// </summary>
        public void SnapToTile(int index)
        {
            if (BoardManager.Instance == null) return;
            Vector3 targetPos = BoardManager.Instance.GetPositionByIndex(index);
            transform.position = targetPos;
        }

        /// <summary>
        /// (Server Only) 移动指定步数
        /// </summary>
        public async UniTask MoveStepsAsync(int steps)
        {
            if (!IsServer) return;

            int totalTiles = BoardManager.Instance.GetTotalTileCount();
            if (totalTiles == 0) return;

            // 逐步移动动画逻辑 (简化版)
            for (int i = 0; i < steps; i++)
            {
                int nextIndex = (CurrentTileIndex.Value + 1) % totalTiles;
                CurrentTileIndex.Value = nextIndex;
                
                // 在服务器端也更新位置，确保物理/触发器正常
                SnapToTile(nextIndex);
                
                // 等待一小段时间模拟跳跃/移动耗时
                await UniTask.Delay(300); 
            }
        }
    }
}
