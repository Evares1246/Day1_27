using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using GameDesign.Gameplay.Map;

namespace GameDesign.Gameplay.Player
{
    /// <summary>
    /// 棋子表现层：监听数据变化并执行 DOTween 动画。
    /// </summary>
    public class PawnVisualizer : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _jumpPower = 1.2f;
        [SerializeField] private float _duration = 0.5f;

        private PlayerBrain _boundBrain;

        public void Initialize(PlayerBrain brain)
        {
            _boundBrain = brain;
            // 订阅大脑中的位置变化事件
            _boundBrain.PawnTileIndex.OnValueChanged += OnMove;
        }

        private void OnMove(int oldIdx, int newIdx)
        {
            if (BoardManager.Instance == null) return;

            Vector3 targetPos = BoardManager.Instance.GetPosition(newIdx);

            // 表现层动画：执行跳跃，不影响逻辑坐标
            transform.DOJump(targetPos, _jumpPower, 1, _duration)
                     .SetEase(Ease.OutQuad);
        }

        private void OnDestroy()
        {
            if (_boundBrain != null)
            {
                _boundBrain.PawnTileIndex.OnValueChanged -= OnMove;
            }
        }
    }
}