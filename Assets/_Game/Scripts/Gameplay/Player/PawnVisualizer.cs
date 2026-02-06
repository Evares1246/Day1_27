using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using GameDesign.Gameplay.Map;

namespace GameDesign.Gameplay.Player
{
    public class PawnVisualizer : NetworkBehaviour
    {
        [SerializeField] private PlayerBrain brain;

        public override void OnNetworkSpawn()
        {
            // 监听大脑中的位置数据变化
            brain.PawnTileIndex.OnValueChanged += OnMove;
        }

        private void OnMove(int oldIdx, int newIdx)
        {
            Vector3 targetPos = BoardManager.Instance.GetPositionByIndex(newIdx);
            // 表现层：执行跳跃动画
            transform.DOJump(targetPos, 1.2f, 1, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}