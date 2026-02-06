using Unity.Netcode;
using UnityEngine;
using DG.Tweening;
using GameDesign.Gameplay.Map;

namespace GameDesign.Gameplay.Player
{
    public class PawnEntity : NetworkBehaviour
    {
        public NetworkVariable<ulong> OwnerId = new NetworkVariable<ulong>();

        private PlayerBrain _boundBrain;

        public override void OnNetworkSpawn()
        {
            BindToBrain();
        }

        private void BindToBrain()
        {
            // 在当前所有已连接的玩家对象中寻找我的主人
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.ClientId == OwnerId.Value)
                {
                    _boundBrain = client.PlayerObject.GetComponent<PlayerBrain>();
                    _boundBrain.PawnTileIndex.OnValueChanged += OnPawnIndexChanged;

                    // 初始位置对齐到轨道
                    UpdateVisualPositionInstant();
                    break;
                }
            }
        }

        private void OnPawnIndexChanged(int oldVal, int newVal)
        {
            Vector3 targetPos = BoardManager.Instance.GetPositionByIndex(newVal);
            transform.DOJump(targetPos, 1.2f, 1, 0.5f).SetEase(Ease.OutQuad);
        }

        private void UpdateVisualPositionInstant()
        {
            if (BoardManager.Instance != null && _boundBrain != null)
            {
                transform.position = BoardManager.Instance.GetPositionByIndex(_boundBrain.PawnTileIndex.Value);
            }
        }
    }
}