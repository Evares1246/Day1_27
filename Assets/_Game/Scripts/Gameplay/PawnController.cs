using UnityEngine;
using Unity.Netcode;
using GameDesign.Gameplay.Map;
using Cysharp.Threading.Tasks;

namespace GameDesign.Gameplay.Player
{
    /// <summary>
    /// 棋子逻辑控制器：仅由服务器执行，处理平滑移动的逻辑步进。
    /// </summary>
    public class PawnController : NetworkBehaviour
    {
        private PlayerBrain _boundBrain;

        public void Initialize(PlayerBrain brain)
        {
            _boundBrain = brain;
        }

        /// <summary>
        /// (Server Only) 执行平滑的逐步移动动画，并更新逻辑索引
        /// </summary>
        public async UniTask MoveStepsAsync(int steps)
        {
            if (!IsServer || _boundBrain == null) return;

            int totalTiles = BoardManager.Instance.TotalTiles;
            if (totalTiles == 0) return;

            // 模拟大富翁逐步跳跃的逻辑
            for (int i = 0; i < steps; i++)
            {
                // 计算下一格索引
                int nextIndex = (_boundBrain.PawnTileIndex.Value + 1) % totalTiles;

                // 更新同步变量（这会自动触发所有客户端 Visualizer 的 OnMove）
                _boundBrain.PawnTileIndex.Value = nextIndex;

                // 服务器本地也更新物理坐标，以防触发地块检测
                transform.position = BoardManager.Instance.GetPosition(nextIndex);

                // 每跳一格等待一段时间，产生视觉上的连续跳跃感
                await UniTask.Delay(400);
            }
        }
    }
}